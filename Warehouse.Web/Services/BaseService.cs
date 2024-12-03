using Microsoft.Extensions.Localization;
using Warehouse.Web.Localization;

namespace Warehouse.Web.Services;

public class BaseService<T>
{
    internal readonly ILogger<T> _logger;
    internal readonly IStringLocalizer<LabelResources> _stringLocalizer;

    public BaseService(ILogger<T> logger, IStringLocalizer<LabelResources> stringLocalizer)
    {
            _logger = logger;
            _stringLocalizer = stringLocalizer;
        }
}