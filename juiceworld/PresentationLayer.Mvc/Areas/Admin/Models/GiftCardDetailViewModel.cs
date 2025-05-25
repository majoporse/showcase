using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Admin.Models;

public class GiftCardDetailViewModel : GiftCardViewModel
{
    public List<CouponCodeViewModel> CouponCodes { get; set; } = [];
}
