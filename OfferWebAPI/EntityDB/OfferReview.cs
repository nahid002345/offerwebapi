//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OfferWebAPI.EntityDB
{
    using System;
    using System.Collections.Generic;
    
    public partial class OfferReview
    {
        public long ID { get; set; }
        public string DeviceID { get; set; }
        public string OfferReviewerName { get; set; }
        public long OfferID { get; set; }
        public Nullable<int> OfferReview1 { get; set; }
        public Nullable<bool> OfferIsFollow { get; set; }
        public Nullable<bool> OfferIsFavorite { get; set; }
        public string OfferReviewComment { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    
        public virtual OffersInfo OffersInfo { get; set; }
    }
}
