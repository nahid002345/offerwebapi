using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using OfferWebAPI.EntityDB;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OfferWebAPI.Models
{
    public class OfferCategoryModel
    {
        public long ID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryIcon { get; set; }
        public bool IsActive { get; set; }
    }

    public class OfferListModel
    {
        public long OfferID { get; set; }
        public string OfferPostName { get; set; }
        public string OfferCategory { get; set; }
        public long OfferCategoryId { get; set; }
        public string OfferLocation { get; set; }
        public long OfferLocationId { get; set; }
        public DateTime OfferStart { get; set; }
        public DateTime OfferEnd { get; set; }
        public string OfferStatus { get; set; }
        public string OfferDiscountAmt { get; set; }
        public string OfferDetail { get; set; }
        public string OfferImagePath { get; set; }
        public bool OfferIsActive { get; set; }
        public Int32 OfferViewCount { get; set; }
        public Int32 OfferReviewCount { get; set; }
        public decimal OfferReviewAverageRating { get; set; }
        public int OfferReviewLikeCount { get; set; }
        public int OfferReviewFollowCount { get; set; }
        public int OfferReviewFollowCompanyCount { get; set; }
        public string OfferCompanyName { get; set; }
        public string OfferCompanyLogoUrl { get; set; }
        public string OfferCompanyContactNo { get; set; }
        public string OfferCompanyEmail { get; set; }
        public string OfferCompanyWebsite { get; set; }
        public int OfferOutletCount { get; set; }

        public bool OfferIsFeatured { get; set; }
        //public List<OfferOutletModel> OfferOutletList { get; set; }
        //public List<OfferReviewModel> OfferReviewList { get; set; }
        
    }


    public class OfferDetailModel
    {
        public long OfferID { get; set; }
        public string OfferPostName { get; set; }
        public string OfferCategory { get; set; }
        public long OfferCategoryId { get; set; }
        public string OfferLocation { get; set; }
        public long OfferLocationId { get; set; }
        public DateTime OfferStart { get; set; }
        public DateTime OfferEnd { get; set; }
        public string OfferStatus { get; set; }
        public string OfferDiscountAmt { get; set; }
        public string OfferDetail { get; set; }
        public string OfferImagePath { get; set; }
        public bool OfferIsActive { get; set; }
        public bool OfferDeviceIsFavorite { get; set; }
        public bool OfferDeviceIsFollow { get; set; }
        public bool OfferDeviceIsReviewed { get; set; }
        public Nullable<int> OfferDevicePostRating { get; set; }
        public Int32 OfferViewCount { get; set; }
        public Int32 OfferReviewCount { get; set; }
        public decimal OfferReviewAverageRating { get; set; }
        public int OfferReviewLikeCount { get; set; }
        public int OfferReviewFollowCount { get; set; }
        public int OfferReviewFollowCompanyCount { get; set; }
        public List<OfferReviewModel> OfferReviewList { get; set; }

        public string OfferCompanyName { get; set; }
        public string OfferCompanyLogoUrl { get; set; }
        public string OfferCompanyContactNo { get; set; }
        public string OfferCompanyEmail { get; set; }
        public string OfferCompanyWebsite { get; set; }

        public bool OfferDeviceIsCompanyFollow { get; set; }

        public List<OfferOutletModel> OfferOutletList { get; set; }
        public int OfferOutletCount { get; set; }
    }

    

    public class OfferReviewModel
    {
        public long OfrReviewID { get; set; }
        public string DeviceID { get; set; }
        public long OfferID { get; set; }
        public string OfferReviewerName { get; set; }
        public Nullable<int> OfferReview1 { get; set; }
        public Nullable<bool> OfferIsFollow { get; set; }
        public Nullable<bool> OfferIsFavorite { get; set; }
        public string OfferReviewComment { get; set; }
    }

    public class OfferOutletModel
    {
        public long OfferOutletD { get; set; }
        public long OfferID { get; set; }
        public string OfferOutletName { get; set; }
        public string OfferOutletAddress { get; set; }
        public string OfferOutletLocation { get; set; }
    }

    public class OfferReviewPostModel
    {
        public bool IsPostedSuccess { get; set; }
        public OfferReview OfferReviewDetails { get; set; }
    }

    public class PostModel
    {
        public bool IsPostedSuccess { get; set; }
        public string PostMsg { get; set; }
    }

    public class FollowedCompanyUnseenOfferCount
    {
        public bool IsCountSuccessful { get; set; }
        public long unseenOfferCount { get; set; }
    }
}