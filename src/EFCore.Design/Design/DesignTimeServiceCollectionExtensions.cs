﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore.Design
{
    /// <summary>
    ///     Extension methods for adding Entity Framework Core design-time services to an <see cref="IServiceCollection" />.
    /// </summary>
    public static class DesignTimeServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the Entity Framework Core design-time services.
        /// </summary>
        /// <param name="services"> The <see cref="IServiceCollection" /> the services will be added to. </param>
        /// <returns> The <paramref name="services" />. This enables chaining additional method calls. </returns>
        public static IServiceCollection AddEntityFrameworkDesignTimeServices([NotNull] this IServiceCollection services)
            => services.AddEntityFrameworkDesignTimeServices(new OperationReporter(handler: null));

        /// <summary>
        ///     Adds the Entity Framework Core design-time services.
        /// </summary>
        /// <param name="services"> The <see cref="IServiceCollection" /> the services will be added to. </param>
        /// <param name="reporter"> Used to report design-time messages. </param>
        /// <returns> The <paramref name="services" />. This enables chaining additional method calls. </returns>
        public static IServiceCollection AddEntityFrameworkDesignTimeServices(
            [NotNull] this IServiceCollection services,
            [NotNull] IOperationReporter reporter)
            => services
                .AddSingleton<AnnotationCodeGeneratorDependencies>()
                .AddSingleton<CoreTypeMapperDependencies>()
                .AddSingleton<CSharpMigrationOperationGeneratorDependencies>()
                .AddSingleton<CSharpMigrationsGeneratorDependencies>()
                .AddSingleton<CSharpSnapshotGeneratorDependencies>()
                .AddSingleton<MigrationsCodeGeneratorDependencies>()
                .AddSingleton<ModelCodeGeneratorDependencies>()
                .AddSingleton<ProviderCodeGeneratorDependencies>()
                .AddSingleton<RelationalTypeMapperDependencies>()
                .AddSingleton<ICandidateNamingService, CandidateNamingService>()
                .AddSingleton<ICSharpDbContextGenerator, CSharpDbContextGenerator>()
                .AddSingleton<ICSharpEntityTypeGenerator, CSharpEntityTypeGenerator>()
                .AddSingleton<ICSharpHelper, CSharpHelper>()
                .AddSingleton<ICSharpMigrationOperationGenerator, CSharpMigrationOperationGenerator>()
                .AddSingleton<ICSharpSnapshotGenerator, CSharpSnapshotGenerator>()
                .AddSingleton<ICSharpUtilities, CSharpUtilities>()
                .AddSingleton(typeof(IDiagnosticsLogger<>), typeof(DiagnosticsLogger<>))
                .AddSingleton<DiagnosticSource>(new DiagnosticListener(DbLoggerCategory.Name))
                .AddSingleton<ILoggingOptions, LoggingOptions>()
                .AddSingleton<IMigrationsCodeGenerator, CSharpMigrationsGenerator>()
                .AddSingleton<IMigrationsCodeGeneratorSelector, MigrationsCodeGeneratorSelector>()
                .AddSingleton<IModelCodeGenerator, CSharpModelGenerator>()
                .AddSingleton<IModelCodeGeneratorSelector, ModelCodeGeneratorSelector>()
                .AddSingleton(reporter)
                .AddSingleton<IPluralizer, NullPluralizer>()
                .AddSingleton<IReverseEngineerScaffolder, ReverseEngineerScaffolder>()
                .AddSingleton<IScaffoldingModelFactory, RelationalScaffoldingModelFactory>()
                .AddSingleton<IScaffoldingTypeMapper, ScaffoldingTypeMapper>()
                .AddTransient<MigrationsScaffolderDependencies>()
                .AddTransient<IMigrationsScaffolder, MigrationsScaffolder>()
                .AddTransient<ISnapshotModelProcessor, SnapshotModelProcessor>()
                .AddLogging(b => b.SetMinimumLevel(LogLevel.Debug).AddProvider(new OperationLoggerProvider(reporter)));

        /// <summary>
        ///     Adds services from the <see cref="DbContext" /> which are used at design time.
        /// </summary>
        /// <param name="services"> The <see cref="IServiceCollection" /> the services will be added to. </param>
        /// <param name="context"> The <see cref="DbContext" /> the services will be added from. </param>
        /// <returns> The <paramref name="services" />. This enables chaining additional method calls. </returns>
        public static IServiceCollection AddDbContextDesignTimeServices(
            [NotNull] this IServiceCollection services,
            [NotNull] DbContext context)
            => services
                .AddTransient(_ => context.GetService<ICurrentDbContext>())
                .AddTransient(_ => context.GetService<IDatabaseProvider>())
                .AddTransient(_ => context.GetService<IDbContextOptions>())
                .AddTransient(_ => context.GetService<IHistoryRepository>())
                .AddTransient(_ => context.GetService<IMigrationsAssembly>())
                .AddTransient(_ => context.GetService<IMigrationsIdGenerator>())
                .AddTransient(_ => context.GetService<IMigrationsModelDiffer>())
                .AddTransient(_ => context.GetService<IMigrator>())
                .AddTransient(_ => context.GetService<IModel>());
    }
}
