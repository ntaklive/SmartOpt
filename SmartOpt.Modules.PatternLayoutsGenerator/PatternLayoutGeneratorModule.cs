using Ninject.Syntax;
using SmartOpt.Modules.PatternLayoutsGenerator.Services;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;

namespace SmartOpt.Modules.PatternLayoutsGenerator
{
    public class PatternLayoutGeneratorModule : ModuleBase
    {
        public override void ConfigureServices(IBindingRoot services)
        {
            services.Bind<IOrderInfoParser>().To<OrderInfoParser>().InSingletonScope();
            services.Bind<IPatternLayoutService>().To<PatternLayoutService>().InSingletonScope();
            services.Bind<IReportService>().To<ReportService>().InSingletonScope();
            services.Bind<IReportExporter>().To<ReportExporter>().InSingletonScope();
            services.Bind<IOrderInfoMerger>().To<OrderInfoMerger>().InSingletonScope();
        }
    }
}