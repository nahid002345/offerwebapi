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
    
    public partial class SysUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SysUser()
        {
            this.OfferCategories = new HashSet<OfferCategory>();
            this.OfferCategories1 = new HashSet<OfferCategory>();
            this.OfferLocations = new HashSet<OfferLocation>();
            this.OfferLocations1 = new HashSet<OfferLocation>();
            this.OfferLocOutletMaps = new HashSet<OfferLocOutletMap>();
            this.OfferLocOutletMaps1 = new HashSet<OfferLocOutletMap>();
            this.OffersInfoes = new HashSet<OffersInfo>();
            this.OffersInfoes1 = new HashSet<OffersInfo>();
        }
    
        public long ID { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public long UserType { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string Website { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogoUrl { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OfferCategory> OfferCategories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OfferCategory> OfferCategories1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OfferLocation> OfferLocations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OfferLocation> OfferLocations1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OfferLocOutletMap> OfferLocOutletMaps { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OfferLocOutletMap> OfferLocOutletMaps1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OffersInfo> OffersInfoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OffersInfo> OffersInfoes1 { get; set; }
        public virtual SysEnum SysEnum { get; set; }
    }
}
