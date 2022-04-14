using System;
using System.Threading.Tasks;

using DocIntel.Core.Models;
using DocIntel.Core.Repositories;
using DocIntel.Core.Settings;
using DocIntel.Core.Utils.Indexation;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Spectre.Console;
using Spectre.Console.Cli;

namespace DocIntel.AdminConsole.Commands.Index
{
    public class IndexSourcesCommand : DocIntelCommand<DocIntelCommandSettings>
    {
        private readonly ISourceIndexingUtility _sourceIndexingUtility;
        private readonly ISourceRepository _sourceRepository;
        private readonly ILogger<IndexSourcesCommand> _logger;

        public IndexSourcesCommand(DocIntelContext context,
            IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
            ApplicationSettings applicationSettings,
            ISourceIndexingUtility sourceIndexingUtility,
            ISourceRepository sourceRepository, ILogger<IndexSourcesCommand> logger) : base(context,
            userClaimsPrincipalFactory, applicationSettings)
        {
            _sourceIndexingUtility = sourceIndexingUtility;
            _sourceRepository = sourceRepository;
            _logger = logger;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, DocIntelCommandSettings settings)
        {
            await base.ExecuteAsync(context, settings);
            
            if (!TryGetAmbientContext(out var ambientContext))
                return 1;

            AnsiConsole.Render(new Markup("[grey]Will remove all sources from the index...[/]"));
            _sourceIndexingUtility.RemoveAll();
            AnsiConsole.Render(new Markup("[green]Done.[/]\n"));

            // TODO Use source repository
            var sources = _context.Sources.Include(_ => _.Documents);

            AnsiConsole.Render(new Markup("[grey]Will index all sources...[/]"));
            foreach (var source in sources)
                try
                {
                    _sourceIndexingUtility.Update(source);
                }
                catch (Exception e)
                {
                    // TODO Use structured logging
                    _logger.LogError($"Could not index source '{source.SourceId}' ({e.Message}).");
                }

            AnsiConsole.Render(new Markup("[green]Done.[/]\n"));

            return 0;
        }
    }
}