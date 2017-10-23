using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OfferWebAPI.EntityDB;
using System.Text;
using Newtonsoft.Json.Linq;
using OfferWebAPI.Models;
using System.IO;
using System.Web;

namespace OfferWebAPI.Controllers
{



    [RoutePrefix("api/Offer")]
    public class OfferController : ApiController
    {
        OFFERDBEntities1 oOFFERDBEntities = new OFFERDBEntities1();
        public static int PageSize = 10;
        public static string baseUrl = @"http://www.discountbuzzbd.info";

        #region private function
        private void SetOfferView(string DeviceId, long OfferId)
        {
            try
            {
                OfferView oOfferView = new OfferView();
                oOfferView.DeviceID = DeviceId;
                oOfferView.OfferID = OfferId;
                oOfferView.OfferIsView = true;
                oOfferView.CreatedOn = DateTime.Now;

                oOFFERDBEntities.OfferViews.Add(oOfferView);
                oOFFERDBEntities.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region category info
        [Route("getCategoryList/")]
        [HttpGet]
        public List<OfferCategoryModel> getCategoryList()
        {
            List<OfferCategoryModel> oDbItem = new List<OfferCategoryModel>();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = false;
            oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            try
            {
                var oCatItem = oOFFERDBEntities.OfferCategories.Where(t => t.IsActive == true).ToList();
                OfferCategoryModel oItem;
                foreach (OfferCategory oModel in oCatItem)
                {
                    oItem = new OfferCategoryModel();
                    oItem.ID = oModel.ID;
                    oItem.CategoryName = oModel.CategoryName;
                    oItem.CategoryIcon =(string.IsNullOrEmpty(oModel.CategoryIcon)) ? string.Empty : baseUrl + oModel.CategoryIcon;
                    oItem.IsActive = oModel.IsActive;
                    oDbItem.Add(oItem);
                }
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }

        [Route("searchByCategory/{CategoryID}")]
        [HttpGet]
        public OfferCategory searchByCategory(string CategoryID)
        {
            OfferCategory oDbItem = new OfferCategory();
            oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;

            try
            {
                oDbItem = oOFFERDBEntities.OfferCategories.FirstOrDefault(t => t.IsActive == true && t.ID.ToString() == CategoryID);
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }


        #endregion

        #region location info
        [Route("getLocationList")]
        [HttpGet]
        public List<OfferLocation> getLocationList()
        {
            List<OfferLocation> oDbItem = new List<OfferLocation>();
            oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;

            try
            {
                oDbItem = oOFFERDBEntities.OfferLocations.Where(t => t.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }




        [Route("categoryByLocation/{LocationID}")]
        [HttpGet]
        public List<OfferCategory> categoryByLocation(Int32 LocationID)
        {
            List<OfferCategory> oDbItem = new List<OfferCategory>();
            oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;

            try
            {
                oDbItem = oOFFERDBEntities.OfferCatLocMaps.Where(t => t.IsActive == true && t.LocationID == LocationID).Select(x => x.OfferCategory).ToList();
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }

        #endregion

        #region Offer List query
        [Route("getAllOfferList")]
        [HttpGet]
        public List<OfferListModel> getAllOfferList()
        {
            List<OfferListModel> oDbItem = new List<OfferListModel>();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;
            //oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            try
            {
                int featureType = Convert.ToInt32(Enumaretion.DBEnumType.OfferFeature);
                long enumhighPriorityId = oOFFERDBEntities.SysEnums.FirstOrDefault(t => t.EnumType == featureType && t.EnumName.ToLower().Contains("high")).ID;
                var oOfferItem = oOFFERDBEntities.OffersInfoes.Where(t => t.IsActive == true).OrderByDescending(x => x.ID).ToList();
                OfferListModel oItem;
                foreach (OffersInfo oModel in oOfferItem)
                {
                    oItem = new OfferListModel();
                    oItem.OfferID = oModel.ID;
                    oItem.OfferPostName = oModel.PostName;
                    oItem.OfferCategory = oModel.OfferCategory.CategoryName;
                    oItem.OfferCategoryId = oModel.OfferCat;
                    oItem.OfferLocation = oModel.OfferLocation.LocationName;
                    oItem.OfferLocationId = oModel.OfferLoc;
                    oItem.OfferStart = oModel.OfferStartDate;
                    oItem.OfferEnd = oModel.OfferEndDate;
                    oItem.OfferStatus = oModel.SysEnum.EnumName;
                    oItem.OfferDiscountAmt = oModel.OfferDiscountAmt;
                    oItem.OfferDetail = oModel.OfferDetails;
                    oItem.OfferImagePath =baseUrl + oModel.OfferImagePath;
                    oItem.OfferIsActive = oModel.IsActive;
                    oItem.OfferViewCount = oModel.OfferViews.Count;
                    oItem.OfferReviewCount = oModel.OfferReviews.Count;
                    oItem.OfferReviewAverageRating = (oModel.OfferReviews.Count > 0) ? (oModel.OfferReviews.Sum(t => t.OfferReview1) / oModel.OfferReviews.Count).GetValueOrDefault(0) : 0;
                    oItem.OfferReviewLikeCount = oModel.OfferReviews.Where(x => x.OfferIsFavorite.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCount = oModel.OfferReviews.Where(x => x.OfferIsFollow.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCompanyCount = oOFFERDBEntities.OfferReviews.Where(x => x.OffersInfo.SysUser.ID == oModel.SysUser.ID).Count();

                    oItem.OfferCompanyName = oModel.SysUser.CompanyName;
                    oItem.OfferCompanyLogoUrl =baseUrl + oModel.SysUser.CompanyLogoUrl;
                    oItem.OfferCompanyContactNo = oModel.SysUser.ContactNo;
                    oItem.OfferCompanyEmail = oModel.SysUser.Email;
                    oItem.OfferCompanyWebsite = oModel.SysUser.Website;
                    oItem.OfferOutletCount = oModel.OfferAvailOutlets.Count;
                    oItem.OfferIsFeatured = (oModel.OfferFeatureVal.GetValueOrDefault(0) == enumhighPriorityId) ? true : false;
                    //oItem.OfferReviewList = new List<OfferReviewModel>();
                    //foreach (OfferReview oReview in oModel.OfferReviews)
                    //{
                    //    OfferReviewModel oOfferReviewMod = new OfferReviewModel();
                    //    oOfferReviewMod.OfrReviewID = oReview.ID;
                    //    oOfferReviewMod.OfferReviewerName = oReview.OfferReviewerName;
                    //    oOfferReviewMod.DeviceID = oReview.DeviceID;
                    //    oOfferReviewMod.OfferIsFavorite = oReview.OfferIsFavorite.GetValueOrDefault(false);

                    //    oOfferReviewMod.OfferIsFollow = oReview.OfferIsFollow.GetValueOrDefault(false);
                    //    oOfferReviewMod.OfferReview1 = oReview.OfferReview1.GetValueOrDefault(0);
                    //    oOfferReviewMod.OfferReviewComment = oReview.OfferReviewComment;

                    //    oItem.OfferReviewList.Add(oOfferReviewMod);
                    //}

                    //oItem.OfferOutletList = new List<OfferOutletModel>();
                    //foreach (OfferAvailOutlet oAvailOutlet in oModel.OfferAvailOutlets)
                    //{
                    //    OfferOutletModel oOfferOutlet = new OfferOutletModel();
                    //    oOfferOutlet.OfferOutletD = oAvailOutlet.OutletID;
                    //    oOfferOutlet.OfferID = oAvailOutlet.OfferId;
                    //    oOfferOutlet.OfferOutletAddress = oAvailOutlet.OfferLocOutletMap.OutletAddress;
                    //    oOfferOutlet.OfferOutletName = oAvailOutlet.OfferLocOutletMap.OutletName;
                    //    oOfferOutlet.OfferOutletLocation = oAvailOutlet.OfferLocOutletMap.OfferLocation.LocationName;



                    //    oItem.OfferOutletList.Add(oOfferOutlet);
                    //}

                    oDbItem.Add(oItem);
                }
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }

        [Route("getOfferList/{DeviceID}")]
        [HttpGet]
        public List<OfferListModel> getOfferList(string DeviceID)
        {
            List<OfferListModel> oDbItem = new List<OfferListModel>();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;
            //oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            try
            {
                int featureType = Convert.ToInt32(Enumaretion.DBEnumType.OfferFeature);
                long enumhighPriorityId = oOFFERDBEntities.SysEnums.FirstOrDefault(t => t.EnumType == featureType && t.EnumName.ToLower().Contains("high")).ID;
                var oOfferItem = oOFFERDBEntities.OffersInfoes.Where(t => t.IsActive == true).OrderByDescending(x => x.ID).ToList();
                OfferListModel oItem;
                foreach (OffersInfo oModel in oOfferItem)
                {
                    oItem = new OfferListModel();
                    oItem.OfferID = oModel.ID;
                    oItem.OfferPostName = oModel.PostName;
                    oItem.OfferCategory = oModel.OfferCategory.CategoryName;
                    oItem.OfferCategoryId = oModel.OfferCat;
                    oItem.OfferLocation = oModel.OfferLocation.LocationName;
                    oItem.OfferLocationId = oModel.OfferLoc;
                    oItem.OfferStart = oModel.OfferStartDate;
                    oItem.OfferEnd = oModel.OfferEndDate;
                    oItem.OfferStatus = oModel.SysEnum.EnumName;
                    oItem.OfferDiscountAmt = oModel.OfferDiscountAmt;
                    oItem.OfferDetail = oModel.OfferDetails;
                    oItem.OfferImagePath = baseUrl + oModel.OfferImagePath;
                    oItem.OfferIsActive = oModel.IsActive;
                    oItem.OfferViewCount = oModel.OfferViews.Count;
                    oItem.OfferReviewCount = oModel.OfferReviews.Count;
                    oItem.OfferReviewAverageRating = (oModel.OfferReviews.Count > 0) ? (oModel.OfferReviews.Sum(t => t.OfferReview1) / oModel.OfferReviews.Count).GetValueOrDefault(0) : 0;
                    oItem.OfferReviewLikeCount = oModel.OfferReviews.Where(x => x.OfferIsFavorite.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCount = oModel.OfferReviews.Where(x => x.OfferIsFollow.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCompanyCount = oOFFERDBEntities.OfferReviews.Where(x => x.OffersInfo.SysUser.ID == oModel.SysUser.ID).Count();

                    oItem.OfferCompanyName = oModel.SysUser.CompanyName;
                    oItem.OfferCompanyLogoUrl =baseUrl + oModel.SysUser.CompanyLogoUrl;
                    oItem.OfferCompanyContactNo = oModel.SysUser.ContactNo;
                    oItem.OfferCompanyEmail = oModel.SysUser.Email;
                    oItem.OfferCompanyWebsite = oModel.SysUser.Website;
                    oItem.OfferOutletCount = oModel.OfferAvailOutlets.Count;
                    oItem.OfferIsFeatured = (oModel.OfferFeatureVal.GetValueOrDefault(0) == enumhighPriorityId) ? true : false;
                    oDbItem.Add(oItem);
                }
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }


        [Route("getOfferListByPage/{DeviceID}/{PageNo}")]
        [HttpGet]
        public List<OfferListModel> getOfferListByPage(string DeviceID, int PageNo)
        {
            List<OfferListModel> oDbItem = new List<OfferListModel>();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;
            //oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            //if (PageNo == null)
            //    PageNo = 1;
            //else


            int itemSkip = PageSize * PageNo;

            PageNo++;
            try
            {
                int featureType = Convert.ToInt32(Enumaretion.DBEnumType.OfferFeature);
                long enumhighPriorityId = oOFFERDBEntities.SysEnums.FirstOrDefault(t => t.EnumType == featureType && t.EnumName.ToLower().Contains("high")).ID;
                var oOfferItem = oOFFERDBEntities.OffersInfoes.Where(t => t.IsActive == true).OrderByDescending(x => x.ID).Skip(itemSkip).Take(PageSize).ToList();
                OfferListModel oItem;
                foreach (OffersInfo oModel in oOfferItem)
                {
                    oItem = new OfferListModel();
                    oItem.OfferID = oModel.ID;
                    oItem.OfferPostName = oModel.PostName;
                    oItem.OfferCategory = oModel.OfferCategory.CategoryName;
                    oItem.OfferCategoryId = oModel.OfferCat;
                    oItem.OfferLocation = oModel.OfferLocation.LocationName;
                    oItem.OfferLocationId = oModel.OfferLoc;
                    oItem.OfferStart = oModel.OfferStartDate;
                    oItem.OfferEnd = oModel.OfferEndDate;
                    oItem.OfferStatus = oModel.SysEnum.EnumName;
                    oItem.OfferDiscountAmt = oModel.OfferDiscountAmt;
                    oItem.OfferDetail = oModel.OfferDetails;
                    oItem.OfferImagePath = baseUrl + oModel.OfferImagePath;
                    oItem.OfferIsActive = oModel.IsActive;
                    oItem.OfferViewCount = oModel.OfferViews.Count;
                    oItem.OfferReviewCount = oModel.OfferReviews.Count;
                    oItem.OfferReviewAverageRating = (oModel.OfferReviews.Count > 0) ? (oModel.OfferReviews.Sum(t => t.OfferReview1) / oModel.OfferReviews.Count).GetValueOrDefault(0) : 0;
                    oItem.OfferReviewLikeCount = oModel.OfferReviews.Where(x => x.OfferIsFavorite.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCount = oModel.OfferReviews.Where(x => x.OfferIsFollow.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCompanyCount = oOFFERDBEntities.OfferReviews.Where(x => x.OffersInfo.SysUser.ID == oModel.SysUser.ID).Count();

                    oItem.OfferCompanyName = oModel.SysUser.CompanyName;
                    oItem.OfferCompanyLogoUrl =baseUrl + oModel.SysUser.CompanyLogoUrl;
                    oItem.OfferCompanyContactNo = oModel.SysUser.ContactNo;
                    oItem.OfferCompanyEmail = oModel.SysUser.Email;
                    oItem.OfferCompanyWebsite = oModel.SysUser.Website;
                    oItem.OfferOutletCount = oModel.OfferAvailOutlets.Count;
                    oItem.OfferIsFeatured = (oModel.OfferFeatureVal.GetValueOrDefault(0) == enumhighPriorityId) ? true : false;

                    oDbItem.Add(oItem);
                }
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }

        [Route("getFollowedOfferListByPage/{DeviceID}/{PageNo}")]
        [HttpGet]
        public List<OfferListModel> getFollowedOfferListByPage(string DeviceID, int PageNo)
        {
            List<OfferListModel> oDbItem = new List<OfferListModel>();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;
            //oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            //if (PageNo == null)
            //    PageNo = 1;
            //else


            int itemSkip = PageSize * PageNo;

            PageNo++;
            try
            {
                int featureType = Convert.ToInt32(Enumaretion.DBEnumType.OfferFeature);
                long enumhighPriorityId = oOFFERDBEntities.SysEnums.FirstOrDefault(t => t.EnumType == featureType && t.EnumName.ToLower().Contains("high")).ID;
                var oOfferItem = oOFFERDBEntities.OfferReviews.Where(t => t.OfferIsFollow == true && t.DeviceID == DeviceID).Select(x => x.OffersInfo).OrderByDescending(x => x.ID).Skip(itemSkip).Distinct().Take(PageSize).ToList();
                OfferListModel oItem;
                foreach (OffersInfo oModel in oOfferItem)
                {
                    oItem = new OfferListModel();
                    oItem.OfferID = oModel.ID;
                    oItem.OfferPostName = oModel.PostName;
                    oItem.OfferCategory = oModel.OfferCategory.CategoryName;
                    oItem.OfferCategoryId = oModel.OfferCat;
                    oItem.OfferLocation = oModel.OfferLocation.LocationName;
                    oItem.OfferLocationId = oModel.OfferLoc;
                    oItem.OfferStart = oModel.OfferStartDate;
                    oItem.OfferEnd = oModel.OfferEndDate;
                    oItem.OfferStatus = oModel.SysEnum.EnumName;
                    oItem.OfferDiscountAmt = oModel.OfferDiscountAmt;
                    oItem.OfferDetail = oModel.OfferDetails;
                    oItem.OfferImagePath = baseUrl + oModel.OfferImagePath;
                    oItem.OfferIsActive = oModel.IsActive;
                    oItem.OfferViewCount = oModel.OfferViews.Count;
                    oItem.OfferReviewCount = oModel.OfferReviews.Count;
                    oItem.OfferReviewAverageRating = (oModel.OfferReviews.Count > 0) ? (oModel.OfferReviews.Sum(t => t.OfferReview1) / oModel.OfferReviews.Count).GetValueOrDefault(0) : 0;
                    oItem.OfferReviewLikeCount = oModel.OfferReviews.Where(x => x.OfferIsFavorite.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCount = oModel.OfferReviews.Where(x => x.OfferIsFollow.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCompanyCount = oOFFERDBEntities.OfferReviews.Where(x => x.OffersInfo.SysUser.ID == oModel.SysUser.ID).Count();

                    oItem.OfferCompanyName = oModel.SysUser.CompanyName;
                    oItem.OfferCompanyLogoUrl =baseUrl + oModel.SysUser.CompanyLogoUrl;
                    oItem.OfferCompanyContactNo = oModel.SysUser.ContactNo;
                    oItem.OfferCompanyEmail = oModel.SysUser.Email;
                    oItem.OfferCompanyWebsite = oModel.SysUser.Website;
                    oItem.OfferOutletCount = oModel.OfferAvailOutlets.Count;
                    oItem.OfferIsFeatured = (oModel.OfferFeatureVal.GetValueOrDefault(0) == enumhighPriorityId) ? true : false;

                    oDbItem.Add(oItem);
                }
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }


        [Route("getFollowedCompnayOfferListByPage/{DeviceID}/{PageNo}")]
        [HttpGet]
        public List<OfferListModel> getFollowedCompnayOfferListByPage(string DeviceID, int PageNo)
        {
            List<OfferListModel> oDbItem = new List<OfferListModel>();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;
            //oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            //if (PageNo == null)
            //    PageNo = 1;
            //else


            int itemSkip = PageSize * PageNo;

            PageNo++;
            try
            {
                int featureType = Convert.ToInt32(Enumaretion.DBEnumType.OfferFeature);
                long enumhighPriorityId = oOFFERDBEntities.SysEnums.FirstOrDefault(t => t.EnumType == featureType && t.EnumName.ToLower().Contains("high")).ID;
                var oFollowedCompanyOffer = oOFFERDBEntities.OfferReviews.Where(t => t.OfferIsFollow == true && t.DeviceID == DeviceID).Select(x => x.OffersInfo.SysUser).Distinct();
                var oOfferItem = oOFFERDBEntities.OffersInfoes.Where(t => t.IsActive == true && oFollowedCompanyOffer.Contains(t.SysUser)).OrderByDescending(x => x.ID).Skip(itemSkip).Distinct().Take(PageSize).ToList();
                OfferListModel oItem;
                foreach (OffersInfo oModel in oOfferItem)
                {
                    oItem = new OfferListModel();
                    oItem.OfferID = oModel.ID;
                    oItem.OfferPostName = oModel.PostName;
                    oItem.OfferCategory = oModel.OfferCategory.CategoryName;
                    oItem.OfferCategoryId = oModel.OfferCat;
                    oItem.OfferLocation = oModel.OfferLocation.LocationName;
                    oItem.OfferLocationId = oModel.OfferLoc;
                    oItem.OfferStart = oModel.OfferStartDate;
                    oItem.OfferEnd = oModel.OfferEndDate;
                    oItem.OfferStatus = oModel.SysEnum.EnumName;
                    oItem.OfferDiscountAmt = oModel.OfferDiscountAmt;
                    oItem.OfferDetail = oModel.OfferDetails;
                    oItem.OfferImagePath = baseUrl + oModel.OfferImagePath;
                    oItem.OfferIsActive = oModel.IsActive;
                    oItem.OfferViewCount = oModel.OfferViews.Count;
                    oItem.OfferReviewCount = oModel.OfferReviews.Count;
                    oItem.OfferReviewAverageRating = (oModel.OfferReviews.Count > 0) ? (oModel.OfferReviews.Sum(t => t.OfferReview1) / oModel.OfferReviews.Count).GetValueOrDefault(0) : 0;
                    oItem.OfferReviewLikeCount = oModel.OfferReviews.Where(x => x.OfferIsFavorite.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCount = oModel.OfferReviews.Where(x => x.OfferIsFollow.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCompanyCount = oOFFERDBEntities.OfferReviews.Where(x => x.OffersInfo.SysUser.ID == oModel.SysUser.ID).Count();

                    oItem.OfferCompanyName = oModel.SysUser.CompanyName;
                    oItem.OfferCompanyLogoUrl = baseUrl + oModel.SysUser.CompanyLogoUrl;
                    oItem.OfferCompanyContactNo = oModel.SysUser.ContactNo;
                    oItem.OfferCompanyEmail = oModel.SysUser.Email;
                    oItem.OfferCompanyWebsite = oModel.SysUser.Website;
                    oItem.OfferOutletCount = oModel.OfferAvailOutlets.Count;
                    oItem.OfferIsFeatured = (oModel.OfferFeatureVal.GetValueOrDefault(0) == enumhighPriorityId) ? true : false;

                    oDbItem.Add(oItem);
                }
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }


        [Route("getFeaturedOfferListByPage/{DeviceID}/{PageNo}")]
        [HttpGet]
        public List<OfferListModel> getFeaturedOfferListByPage(string DeviceID, int PageNo)
        {
            List<OfferListModel> oDbItem = new List<OfferListModel>();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;
            //oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            //if (PageNo == null)
            //    PageNo = 1;
            //else


            int itemSkip = PageSize * PageNo;

            PageNo++;
            try
            {
                int featureType = Convert.ToInt32(Enumaretion.DBEnumType.OfferFeature);
                long enumhighPriorityId = oOFFERDBEntities.SysEnums.FirstOrDefault(t => t.EnumType == featureType && t.EnumName.ToLower().Contains("high")).ID;
                var oOfferItem = oOFFERDBEntities.OffersInfoes.Where(t => t.IsActive == true && t.OfferFeatureVal == enumhighPriorityId).OrderByDescending(x => x.ID).Skip(itemSkip).Take(PageSize).ToList();
                OfferListModel oItem;
                foreach (OffersInfo oModel in oOfferItem)
                {
                    oItem = new OfferListModel();
                    oItem.OfferID = oModel.ID;
                    oItem.OfferPostName = oModel.PostName;
                    oItem.OfferCategory = oModel.OfferCategory.CategoryName;
                    oItem.OfferCategoryId = oModel.OfferCat;
                    oItem.OfferLocation = oModel.OfferLocation.LocationName;
                    oItem.OfferLocationId = oModel.OfferLoc;
                    oItem.OfferStart = oModel.OfferStartDate;
                    oItem.OfferEnd = oModel.OfferEndDate;
                    oItem.OfferStatus = oModel.SysEnum.EnumName;
                    oItem.OfferDiscountAmt = oModel.OfferDiscountAmt;
                    oItem.OfferDetail = oModel.OfferDetails;
                    oItem.OfferImagePath = baseUrl + oModel.OfferImagePath;
                    oItem.OfferIsActive = oModel.IsActive;
                    oItem.OfferViewCount = oModel.OfferViews.Count;
                    oItem.OfferReviewCount = oModel.OfferReviews.Count;
                    oItem.OfferReviewAverageRating = (oModel.OfferReviews.Count > 0) ? (oModel.OfferReviews.Sum(t => t.OfferReview1) / oModel.OfferReviews.Count).GetValueOrDefault(0) : 0;
                    oItem.OfferReviewLikeCount = oModel.OfferReviews.Where(x => x.OfferIsFavorite.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCount = oModel.OfferReviews.Where(x => x.OfferIsFollow.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCompanyCount = oOFFERDBEntities.OfferReviews.Where(x => x.OffersInfo.SysUser.ID == oModel.SysUser.ID).Count();

                    oItem.OfferCompanyName = oModel.SysUser.CompanyName;
                    oItem.OfferCompanyLogoUrl =baseUrl + oModel.SysUser.CompanyLogoUrl;
                    oItem.OfferCompanyContactNo = oModel.SysUser.ContactNo;
                    oItem.OfferCompanyEmail = oModel.SysUser.Email;
                    oItem.OfferCompanyWebsite = oModel.SysUser.Website;
                    oItem.OfferOutletCount = oModel.OfferAvailOutlets.Count;
                    oItem.OfferIsFeatured = (oModel.OfferFeatureVal.GetValueOrDefault(0) == enumhighPriorityId) ? true : false;
                    oDbItem.Add(oItem);
                }
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }

        [Route("getOfferListByCategoryID/{CategoryID}")]
        [HttpGet]
        public List<OfferListModel> getOfferListByCategoryID(Int32 CategoryID)
        {
            List<OfferListModel> oDbItem = new List<OfferListModel>();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;
            //oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            try
            {
                int featureType = Convert.ToInt32(Enumaretion.DBEnumType.OfferFeature);
                long enumhighPriorityId = oOFFERDBEntities.SysEnums.FirstOrDefault(t => t.EnumType == featureType && t.EnumName.ToLower().Contains("high")).ID;
                var oOfferItem = oOFFERDBEntities.OffersInfoes.Where(t => t.IsActive == true && t.OfferCat == CategoryID).ToList();
                OfferListModel oItem;
                foreach (OffersInfo oModel in oOfferItem)
                {
                    oItem = new OfferListModel();
                    oItem.OfferID = oModel.ID;
                    oItem.OfferPostName = oModel.PostName;
                    oItem.OfferCategory = oModel.OfferCategory.CategoryName;
                    oItem.OfferCategoryId = oModel.OfferCat;
                    oItem.OfferLocation = oModel.OfferLocation.LocationName;
                    oItem.OfferLocationId = oModel.OfferLoc;
                    oItem.OfferStart = oModel.OfferStartDate;
                    oItem.OfferEnd = oModel.OfferEndDate;
                    oItem.OfferStatus = oModel.SysEnum.EnumName;
                    oItem.OfferDiscountAmt = oModel.OfferDiscountAmt;
                    oItem.OfferDetail = oModel.OfferDetails;
                    oItem.OfferImagePath = baseUrl + oModel.OfferImagePath;
                    oItem.OfferIsActive = oModel.IsActive;
                    oItem.OfferViewCount = oModel.OfferViews.Count;
                    oItem.OfferReviewCount = oModel.OfferReviews.Count;
                    oItem.OfferReviewAverageRating = (oModel.OfferReviews.Count > 0) ? (oModel.OfferReviews.Sum(t => t.OfferReview1) / oModel.OfferReviews.Count).GetValueOrDefault(0) : 0;
                    oItem.OfferReviewLikeCount = oModel.OfferReviews.Where(x => x.OfferIsFavorite.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCount = oModel.OfferReviews.Where(x => x.OfferIsFollow.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCompanyCount = oOFFERDBEntities.OfferReviews.Where(x => x.OffersInfo.SysUser.ID == oModel.SysUser.ID).Count();

                    oItem.OfferCompanyName = oModel.SysUser.CompanyName;
                    oItem.OfferCompanyLogoUrl =baseUrl + oModel.SysUser.CompanyLogoUrl;
                    oItem.OfferCompanyContactNo = oModel.SysUser.ContactNo;
                    oItem.OfferCompanyEmail = oModel.SysUser.Email;
                    oItem.OfferCompanyWebsite = oModel.SysUser.Website;
                    oItem.OfferOutletCount = oModel.OfferAvailOutlets.Count;
                    oItem.OfferIsFeatured = (oModel.OfferFeatureVal.GetValueOrDefault(0) == enumhighPriorityId) ? true : false;
                    oDbItem.Add(oItem);
                }
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }


        [Route("getOfferListPageByCategoryID/{CategoryID}/{PageNo}")]
        [HttpGet]
        public List<OfferListModel> getOfferListPageByCategoryID(Int32 CategoryID, int PageNo)
        {
            List<OfferListModel> oDbItem = new List<OfferListModel>();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;
            //oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;

            int itemSkip = PageSize * PageNo;
            PageNo++;
            try
            {
                int featureType = Convert.ToInt32(Enumaretion.DBEnumType.OfferFeature);
                long enumhighPriorityId = oOFFERDBEntities.SysEnums.FirstOrDefault(t => t.EnumType == featureType && t.EnumName.ToLower().Contains("high")).ID;
                var oOfferItem = oOFFERDBEntities.OffersInfoes.Where(t => t.IsActive == true && t.OfferCat == CategoryID).OrderByDescending(x => x.ID).Skip(itemSkip).Take(PageSize).ToList();
                OfferListModel oItem;
                foreach (OffersInfo oModel in oOfferItem)
                {
                    oItem = new OfferListModel();
                    oItem.OfferID = oModel.ID;
                    oItem.OfferPostName = oModel.PostName;
                    oItem.OfferCategory = oModel.OfferCategory.CategoryName;
                    oItem.OfferCategoryId = oModel.OfferCat;
                    oItem.OfferLocation = oModel.OfferLocation.LocationName;
                    oItem.OfferLocationId = oModel.OfferLoc;
                    oItem.OfferStart = oModel.OfferStartDate;
                    oItem.OfferEnd = oModel.OfferEndDate;
                    oItem.OfferStatus = oModel.SysEnum.EnumName;
                    oItem.OfferDiscountAmt = oModel.OfferDiscountAmt;
                    oItem.OfferDetail = oModel.OfferDetails;
                    oItem.OfferImagePath = baseUrl + oModel.OfferImagePath;
                    oItem.OfferIsActive = oModel.IsActive;
                    oItem.OfferViewCount = oModel.OfferViews.Count;
                    oItem.OfferReviewCount = oModel.OfferReviews.Count;
                    oItem.OfferReviewAverageRating = (oModel.OfferReviews.Count > 0) ? (oModel.OfferReviews.Sum(t => t.OfferReview1) / oModel.OfferReviews.Count).GetValueOrDefault(0) : 0;
                    oItem.OfferReviewLikeCount = oModel.OfferReviews.Where(x => x.OfferIsFavorite.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCount = oModel.OfferReviews.Where(x => x.OfferIsFollow.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCompanyCount = oOFFERDBEntities.OfferReviews.Where(x => x.OffersInfo.SysUser.ID == oModel.SysUser.ID).Count();

                    oItem.OfferCompanyName = oModel.SysUser.CompanyName;
                    oItem.OfferCompanyLogoUrl =baseUrl + oModel.SysUser.CompanyLogoUrl;
                    oItem.OfferCompanyContactNo = oModel.SysUser.ContactNo;
                    oItem.OfferCompanyEmail = oModel.SysUser.Email;
                    oItem.OfferCompanyWebsite = oModel.SysUser.Website;
                    oItem.OfferOutletCount = oModel.OfferAvailOutlets.Count;
                    oItem.OfferIsFeatured = (oModel.OfferFeatureVal.GetValueOrDefault(0) == enumhighPriorityId) ? true : false;
                    oDbItem.Add(oItem);
                }
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }


        [Route("mostReviewedOffer")]
        [HttpGet]
        public List<OfferListModel> mostReviewedOffer()
        {
            List<OfferListModel> oDbItem = new List<OfferListModel>();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;

            try
            {
                int featureType = Convert.ToInt32(Enumaretion.DBEnumType.OfferFeature);
                long enumhighPriorityId = oOFFERDBEntities.SysEnums.FirstOrDefault(t => t.EnumType == featureType && t.EnumName.ToLower().Contains("high")).ID;
                var oOfferItem = oOFFERDBEntities.OffersInfoes.Where(t => t.IsActive == true).ToList().OrderByDescending(s => s.OfferReviews.Count);
                OfferListModel oItem;
                foreach (OffersInfo oModel in oOfferItem)
                {
                    oItem = new OfferListModel();
                    oItem.OfferID = oModel.ID;
                    oItem.OfferPostName = oModel.PostName;
                    oItem.OfferCategory = oModel.OfferCategory.CategoryName;
                    oItem.OfferCategoryId = oModel.OfferCat;
                    oItem.OfferLocation = oModel.OfferLocation.LocationName;
                    oItem.OfferLocationId = oModel.OfferLoc;
                    oItem.OfferStart = oModel.OfferStartDate;
                    oItem.OfferEnd = oModel.OfferEndDate;
                    oItem.OfferStatus = oModel.SysEnum.EnumName;
                    oItem.OfferDiscountAmt = oModel.OfferDiscountAmt;
                    oItem.OfferDetail = oModel.OfferDetails;
                    oItem.OfferImagePath = baseUrl + oModel.OfferImagePath;
                    oItem.OfferIsActive = oModel.IsActive;
                    oItem.OfferViewCount = oModel.OfferViews.Count;
                    oItem.OfferReviewCount = oModel.OfferReviews.Count;
                    oItem.OfferReviewAverageRating = (oModel.OfferReviews.Count > 0) ? (oModel.OfferReviews.Sum(t => t.OfferReview1) / oModel.OfferReviews.Count).GetValueOrDefault(0) : 0;
                    oItem.OfferReviewLikeCount = oModel.OfferReviews.Where(x => x.OfferIsFavorite.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCount = oModel.OfferReviews.Where(x => x.OfferIsFollow.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCompanyCount = oOFFERDBEntities.OfferReviews.Where(x => x.OffersInfo.SysUser.ID == oModel.SysUser.ID).Count();

                    oItem.OfferCompanyName = oModel.SysUser.CompanyName;
                    oItem.OfferCompanyLogoUrl =baseUrl + oModel.SysUser.CompanyLogoUrl;
                    oItem.OfferCompanyContactNo = oModel.SysUser.ContactNo;
                    oItem.OfferCompanyEmail = oModel.SysUser.Email;
                    oItem.OfferCompanyWebsite = oModel.SysUser.Website;
                    oItem.OfferOutletCount = oModel.OfferAvailOutlets.Count;
                    oItem.OfferIsFeatured = (oModel.OfferFeatureVal.GetValueOrDefault(0) == enumhighPriorityId) ? true : false;
                    oDbItem.Add(oItem);
                }
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }


        [Route("mostReviewedOfferByPage/{PageNo}")]
        [HttpGet]
        public List<OfferListModel> mostReviewedOfferByPage(int PageNo)
        {
            List<OfferListModel> oDbItem = new List<OfferListModel>();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;


            int itemSkip = PageSize * PageNo;
            PageNo++;
            try
            {
                int featureType = Convert.ToInt32(Enumaretion.DBEnumType.OfferFeature);
                long enumhighPriorityId = oOFFERDBEntities.SysEnums.FirstOrDefault(t => t.EnumType == featureType && t.EnumName.ToLower().Contains("high")).ID;
                var oOfferItem = oOFFERDBEntities.OffersInfoes.Where(t => t.IsActive == true).OrderByDescending(x=>x.OfferReviews.Count(p=>p.OfferID == x.ID)).Skip(itemSkip).Take(PageSize).ToList();
                OfferListModel oItem;
                foreach (OffersInfo oModel in oOfferItem)
                {
                    oItem = new OfferListModel();
                    oItem.OfferID = oModel.ID;
                    oItem.OfferPostName = oModel.PostName;
                    oItem.OfferCategory = oModel.OfferCategory.CategoryName;
                    oItem.OfferCategoryId = oModel.OfferCat;
                    oItem.OfferLocation = oModel.OfferLocation.LocationName;
                    oItem.OfferLocationId = oModel.OfferLoc;
                    oItem.OfferStart = oModel.OfferStartDate;
                    oItem.OfferEnd = oModel.OfferEndDate;
                    oItem.OfferStatus = oModel.SysEnum.EnumName;
                    oItem.OfferDiscountAmt = oModel.OfferDiscountAmt;
                    oItem.OfferDetail = oModel.OfferDetails;
                    oItem.OfferImagePath = baseUrl + oModel.OfferImagePath;
                    oItem.OfferIsActive = oModel.IsActive;
                    oItem.OfferViewCount = oModel.OfferViews.Count;
                    oItem.OfferReviewCount = oModel.OfferReviews.Count;
                    oItem.OfferReviewAverageRating = (oModel.OfferReviews.Count > 0) ? (oModel.OfferReviews.Sum(t => t.OfferReview1) / oModel.OfferReviews.Count).GetValueOrDefault(0) : 0;
                    oItem.OfferReviewLikeCount = oModel.OfferReviews.Where(x => x.OfferIsFavorite.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCount = oModel.OfferReviews.Where(x => x.OfferIsFollow.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCompanyCount = oOFFERDBEntities.OfferReviews.Where(x => x.OffersInfo.SysUser.ID == oModel.SysUser.ID).Count();

                    oItem.OfferCompanyName = oModel.SysUser.CompanyName;
                    oItem.OfferCompanyLogoUrl =baseUrl + oModel.SysUser.CompanyLogoUrl;
                    oItem.OfferCompanyContactNo = oModel.SysUser.ContactNo;
                    oItem.OfferCompanyEmail = oModel.SysUser.Email;
                    oItem.OfferCompanyWebsite = oModel.SysUser.Website;
                    oItem.OfferOutletCount = oModel.OfferAvailOutlets.Count;
                    oItem.OfferIsFeatured = (oModel.OfferFeatureVal.GetValueOrDefault(0) == enumhighPriorityId) ? true : false;

                    oDbItem.Add(oItem);
                }
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }



        [Route("topViewedOfferByPage/{PageNo}")]
        [HttpGet]
        public List<OfferListModel> topViewedOfferByPage(int PageNo)
        {
            List<OfferListModel> oDbItem = new List<OfferListModel>();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;


            int itemSkip = PageSize * PageNo;
            PageNo++;

            try
            {
                int featureType = Convert.ToInt32(Enumaretion.DBEnumType.OfferFeature);
                long enumhighPriorityId = oOFFERDBEntities.SysEnums.FirstOrDefault(t => t.EnumType == featureType && t.EnumName.ToLower().Contains("high")).ID;
                var oOfferItem = oOFFERDBEntities.OffersInfoes.Where(t => t.IsActive == true).OrderByDescending(x => x.OfferViews.Count).Skip(itemSkip).Take(PageSize).ToList();
                OfferListModel oItem;
                foreach (OffersInfo oModel in oOfferItem)
                {
                    oItem = new OfferListModel();
                    oItem.OfferID = oModel.ID;
                    oItem.OfferPostName = oModel.PostName;
                    oItem.OfferCategory = oModel.OfferCategory.CategoryName;
                    oItem.OfferCategoryId = oModel.OfferCat;
                    oItem.OfferLocation = oModel.OfferLocation.LocationName;
                    oItem.OfferLocationId = oModel.OfferLoc;
                    oItem.OfferStart = oModel.OfferStartDate;
                    oItem.OfferEnd = oModel.OfferEndDate;
                    oItem.OfferStatus = oModel.SysEnum.EnumName;
                    oItem.OfferDiscountAmt = oModel.OfferDiscountAmt;
                    oItem.OfferDetail = oModel.OfferDetails;
                    oItem.OfferImagePath = baseUrl + oModel.OfferImagePath; 
                    oItem.OfferIsActive = oModel.IsActive;
                    oItem.OfferViewCount = oModel.OfferViews.Count;
                    oItem.OfferReviewCount = oModel.OfferReviews.Count;
                    oItem.OfferReviewAverageRating = (oModel.OfferReviews.Count > 0) ? (oModel.OfferReviews.Sum(t => t.OfferReview1) / oModel.OfferReviews.Count).GetValueOrDefault(0) : 0;
                    oItem.OfferReviewLikeCount = oModel.OfferReviews.Where(x => x.OfferIsFavorite.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCount = oModel.OfferReviews.Where(x => x.OfferIsFollow.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCompanyCount = oOFFERDBEntities.OfferReviews.Where(x => x.OffersInfo.SysUser.ID == oModel.SysUser.ID).Count();

                    oItem.OfferCompanyName = oModel.SysUser.CompanyName;
                    oItem.OfferCompanyLogoUrl = baseUrl + oModel.SysUser.CompanyLogoUrl;
                    oItem.OfferCompanyContactNo = oModel.SysUser.ContactNo;
                    oItem.OfferCompanyEmail = oModel.SysUser.Email;
                    oItem.OfferCompanyWebsite = oModel.SysUser.Website;
                    oItem.OfferOutletCount = oModel.OfferAvailOutlets.Count;
                    oItem.OfferIsFeatured = (oModel.OfferFeatureVal.GetValueOrDefault(0) == enumhighPriorityId) ? true : false;

                    oDbItem.Add(oItem);
                }
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }

        #endregion

        #region Offer details
        [Route("getOfferDetails/{DeviceID}/{OfferID}")]
        [HttpGet]
        public OfferDetailModel getOfferDetails(string DeviceID, long OfferID)
        {
            OfferView oOfferView = new OfferView();
            OfferDetailModel oDbItem = new OfferDetailModel();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;
            //oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            try
            {
                var oOfferItem = oOFFERDBEntities.OffersInfoes.FirstOrDefault(t => t.IsActive == true && t.ID == OfferID);


                if (oOfferItem != null)
                {

                    oDbItem.OfferID = oOfferItem.ID;
                    oDbItem.OfferPostName = oOfferItem.PostName;
                    oDbItem.OfferCategory = oOfferItem.OfferCategory.CategoryName;
                    oDbItem.OfferCategoryId = oOfferItem.OfferCat;
                    oDbItem.OfferLocation = oOfferItem.OfferLocation.LocationName;
                    oDbItem.OfferLocationId = oOfferItem.OfferLoc;
                    oDbItem.OfferStart = oOfferItem.OfferStartDate;
                    oDbItem.OfferEnd = oOfferItem.OfferEndDate;
                    oDbItem.OfferStatus = oOfferItem.SysEnum.EnumName;
                    oDbItem.OfferDiscountAmt = oOfferItem.OfferDiscountAmt;
                    oDbItem.OfferDetail = oOfferItem.OfferDetails;
                    oDbItem.OfferImagePath =baseUrl + oOfferItem.OfferImagePath;
                    oDbItem.OfferIsActive = oOfferItem.IsActive;


                    var oFollowCompanyId = oOFFERDBEntities.OfferReviews.Where(t => t.DeviceID == DeviceID).Select(x => x.OffersInfo.SysUser.CompanyName.ToLower().Trim()).Distinct().ToList();
                    if (oFollowCompanyId != null)
                        oDbItem.OfferDeviceIsCompanyFollow = (oFollowCompanyId.Exists(t => t.Equals(oOfferItem.SysUser.CompanyName.ToLower().Trim()))) ? true : false;

                    if (oOfferItem.OfferReviews.FirstOrDefault(t => t.DeviceID == DeviceID) != null)
                    {
                        oDbItem.OfferDeviceIsReviewed = true;
                        oDbItem.OfferDeviceIsFavorite = oOfferItem.OfferReviews.FirstOrDefault(t => t.DeviceID == DeviceID).OfferIsFavorite.GetValueOrDefault(false);
                        oDbItem.OfferDeviceIsFollow = oOfferItem.OfferReviews.FirstOrDefault(t => t.DeviceID == DeviceID).OfferIsFollow.GetValueOrDefault(false);
                        oDbItem.OfferDevicePostRating = oOfferItem.OfferReviews.FirstOrDefault(t => t.DeviceID == DeviceID).OfferReview1.GetValueOrDefault(0);

                    }

                    oDbItem.OfferViewCount = oOfferItem.OfferViews.Count;
                    oDbItem.OfferReviewCount = oOfferItem.OfferReviews.Count;
                    oDbItem.OfferReviewAverageRating = (oOfferItem.OfferReviews.Count > 0) ? (oOfferItem.OfferReviews.Sum(t => t.OfferReview1) / oOfferItem.OfferReviews.Count).GetValueOrDefault(0) : 0;
                    oDbItem.OfferReviewLikeCount = oOfferItem.OfferReviews.Where(x => x.OfferIsFavorite.GetValueOrDefault(false) == true).Count();
                    oDbItem.OfferReviewFollowCount = oOfferItem.OfferReviews.Where(x => x.OfferIsFollow.GetValueOrDefault(false) == true).Count();
                    oDbItem.OfferReviewFollowCompanyCount = oOFFERDBEntities.OfferReviews.Where(x => x.OffersInfo.SysUser.ID == oOfferItem.SysUser.ID).Count();

                    oDbItem.OfferCompanyName = oOfferItem.SysUser.CompanyName;
                    oDbItem.OfferCompanyLogoUrl = baseUrl + oOfferItem.SysUser.CompanyLogoUrl;
                    oDbItem.OfferCompanyContactNo = oOfferItem.SysUser.ContactNo;
                    oDbItem.OfferCompanyEmail = oOfferItem.SysUser.Email;
                    oDbItem.OfferCompanyWebsite = oOfferItem.SysUser.Website;
                    oDbItem.OfferOutletCount = oOfferItem.OfferAvailOutlets.Count;



                    oDbItem.OfferReviewList = new List<OfferReviewModel>();
                    foreach (OfferReview oReview in oOfferItem.OfferReviews)
                    {
                        OfferReviewModel oOfferReviewMod = new OfferReviewModel();
                        oOfferReviewMod.OfrReviewID = oReview.ID;
                        oOfferReviewMod.OfferReviewerName = oReview.OfferReviewerName;
                        oOfferReviewMod.OfferID = oReview.OfferID;
                        oOfferReviewMod.DeviceID = oReview.DeviceID;
                        oOfferReviewMod.OfferIsFavorite = oReview.OfferIsFavorite.GetValueOrDefault(false);

                        oOfferReviewMod.OfferIsFollow = oReview.OfferIsFollow.GetValueOrDefault(false);
                        oOfferReviewMod.OfferReview1 = oReview.OfferReview1.GetValueOrDefault(0);
                        oOfferReviewMod.OfferReviewComment = oReview.OfferReviewComment;

                        oDbItem.OfferReviewList.Add(oOfferReviewMod);
                    }

                    oDbItem.OfferOutletList = new List<OfferOutletModel>();
                    foreach (OfferAvailOutlet oAvailOutlet in oOfferItem.OfferAvailOutlets)
                    {
                        OfferOutletModel oOfferOutlet = new OfferOutletModel();
                        oOfferOutlet.OfferOutletD = oAvailOutlet.OutletID;
                        oOfferOutlet.OfferID = oAvailOutlet.OfferId;
                        oOfferOutlet.OfferOutletAddress = oAvailOutlet.OfferLocOutletMap.OutletAddress;
                        oOfferOutlet.OfferOutletName = oAvailOutlet.OfferLocOutletMap.OutletName;
                        oOfferOutlet.OfferOutletLocation = oAvailOutlet.OfferLocOutletMap.OfferLocation.LocationName;



                        oDbItem.OfferOutletList.Add(oOfferOutlet);
                    }
                    SetOfferView(DeviceID, OfferID);

                }

            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }


        [Route("searchOfferByKeyword/{KeyWord}")]
        [HttpGet]
        public List<OfferListModel> searchOfferByKeyword(string KeyWord)
        {
            List<OfferListModel> oDbItem = new List<OfferListModel>();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;
            //oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            try
            {
                int featureType = Convert.ToInt32(Enumaretion.DBEnumType.OfferFeature);
                long enumhighPriorityId = oOFFERDBEntities.SysEnums.FirstOrDefault(t => t.EnumType == featureType && t.EnumName.ToLower().Contains("high")).ID;
                var oOfferItem = oOFFERDBEntities.OffersInfoes.Where(t => t.IsActive == true && (t.PostName.Contains(KeyWord) || 
                t.OfferDiscountAmt.Contains(KeyWord) ||
                t.OfferAvailOutlets.FirstOrDefault(x=>x.OfferLocOutletMap.OutletAddress.Contains(KeyWord)) != null ||
                t.OfferCategory.CategoryName.Contains(KeyWord) 
                || t.OfferLocation.LocationName.Contains(KeyWord))).ToList();
                OfferListModel oItem;
                foreach (OffersInfo oModel in oOfferItem)
                {
                    oItem = new OfferListModel();
                    oItem.OfferID = oModel.ID;
                    oItem.OfferPostName = oModel.PostName;
                    oItem.OfferCategory = oModel.OfferCategory.CategoryName;
                    oItem.OfferCategoryId = oModel.OfferCat;
                    oItem.OfferLocation = oModel.OfferLocation.LocationName;
                    oItem.OfferLocationId = oModel.OfferLoc;
                    oItem.OfferStart = oModel.OfferStartDate;
                    oItem.OfferEnd = oModel.OfferEndDate;
                    oItem.OfferStatus = oModel.SysEnum.EnumName;
                    oItem.OfferDiscountAmt = oModel.OfferDiscountAmt;
                    oItem.OfferDetail = oModel.OfferDetails;
                    oItem.OfferImagePath = baseUrl + oModel.OfferImagePath;
                    oItem.OfferIsActive = oModel.IsActive;
                    oItem.OfferViewCount = oModel.OfferViews.Count;
                    oItem.OfferReviewCount = oModel.OfferReviews.Count;
                    oItem.OfferReviewAverageRating = (oModel.OfferReviews.Count > 0) ? (oModel.OfferReviews.Sum(t => t.OfferReview1) / oModel.OfferReviews.Count).GetValueOrDefault(0) : 0;
                    oItem.OfferReviewLikeCount = oModel.OfferReviews.Where(x => x.OfferIsFavorite.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCount = oModel.OfferReviews.Where(x => x.OfferIsFollow.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCompanyCount = oOFFERDBEntities.OfferReviews.Where(x => x.OffersInfo.SysUser.ID == oModel.SysUser.ID).Count();

                    oItem.OfferCompanyName = oModel.SysUser.CompanyName;
                    oItem.OfferCompanyLogoUrl =baseUrl + oModel.SysUser.CompanyLogoUrl;
                    oItem.OfferCompanyContactNo = oModel.SysUser.ContactNo;
                    oItem.OfferCompanyEmail = oModel.SysUser.Email;
                    oItem.OfferCompanyWebsite = oModel.SysUser.Website;
                    oItem.OfferOutletCount = oModel.OfferAvailOutlets.Count;
                    oItem.OfferIsFeatured = (oModel.OfferFeatureVal.GetValueOrDefault(0) == enumhighPriorityId) ? true : false;

                    oDbItem.Add(oItem);
                }
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }



        [Route("searchOfferByKeywordCategoryId/{KeyWord}/{CategoryId}")]
        [HttpGet]
        public List<OfferListModel> searchOfferByKeywordCategoryId(string KeyWord,long CategoryId)
        {
            List<OfferListModel> oDbItem = new List<OfferListModel>();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;
            //oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            try
            {
                int featureType = Convert.ToInt32(Enumaretion.DBEnumType.OfferFeature);
                long enumhighPriorityId = oOFFERDBEntities.SysEnums.FirstOrDefault(t => t.EnumType == featureType && t.EnumName.ToLower().Contains("high")).ID;
                var oOfferItem = oOFFERDBEntities.OffersInfoes.Where(t => t.IsActive == true && t.OfferCat == CategoryId && ((t.PostName.Contains(KeyWord) ||
                t.OfferDiscountAmt.Contains(KeyWord) ||
                t.OfferAvailOutlets.FirstOrDefault(x => x.OfferLocOutletMap.OutletAddress.Contains(KeyWord)) != null 
                || t.OfferLocation.LocationName.Contains(KeyWord)))).ToList();
                OfferListModel oItem;
                foreach (OffersInfo oModel in oOfferItem)
                {
                    oItem = new OfferListModel();
                    oItem.OfferID = oModel.ID;
                    oItem.OfferPostName = oModel.PostName;
                    oItem.OfferCategory = oModel.OfferCategory.CategoryName;
                    oItem.OfferCategoryId = oModel.OfferCat;
                    oItem.OfferLocation = oModel.OfferLocation.LocationName;
                    oItem.OfferLocationId = oModel.OfferLoc;
                    oItem.OfferStart = oModel.OfferStartDate;
                    oItem.OfferEnd = oModel.OfferEndDate;
                    oItem.OfferStatus = oModel.SysEnum.EnumName;
                    oItem.OfferDiscountAmt = oModel.OfferDiscountAmt;
                    oItem.OfferDetail = oModel.OfferDetails;
                    oItem.OfferImagePath = baseUrl + oModel.OfferImagePath;
                    oItem.OfferIsActive = oModel.IsActive;
                    oItem.OfferViewCount = oModel.OfferViews.Count;
                    oItem.OfferReviewCount = oModel.OfferReviews.Count;
                    oItem.OfferReviewAverageRating = (oModel.OfferReviews.Count > 0) ? (oModel.OfferReviews.Sum(t => t.OfferReview1) / oModel.OfferReviews.Count).GetValueOrDefault(0) : 0;
                    oItem.OfferReviewLikeCount = oModel.OfferReviews.Where(x => x.OfferIsFavorite.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCount = oModel.OfferReviews.Where(x => x.OfferIsFollow.GetValueOrDefault(false) == true).Count();
                    oItem.OfferReviewFollowCompanyCount = oOFFERDBEntities.OfferReviews.Where(x => x.OffersInfo.SysUser.ID == oModel.SysUser.ID).Count();

                    oItem.OfferCompanyName = oModel.SysUser.CompanyName;
                    oItem.OfferCompanyLogoUrl = baseUrl + oModel.SysUser.CompanyLogoUrl;
                    oItem.OfferCompanyContactNo = oModel.SysUser.ContactNo;
                    oItem.OfferCompanyEmail = oModel.SysUser.Email;
                    oItem.OfferCompanyWebsite = oModel.SysUser.Website;
                    oItem.OfferOutletCount = oModel.OfferAvailOutlets.Count;
                    oItem.OfferIsFeatured = (oModel.OfferFeatureVal.GetValueOrDefault(0) == enumhighPriorityId) ? true : false;

                    oDbItem.Add(oItem);
                }
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }
        #endregion


        #region User Operation

        [Route("userFeedback/{DeviceID}/{UserName}/{Email}/{Comment}")]
        [HttpGet]
        public PostModel userFeedback(string DeviceID, string UserName, string Email, string Comment)
        {
            PostModel oPostItem = new PostModel();
            
            try
            {
                OfferFeedback oFeedBack= new OfferFeedback();
                oFeedBack.DeviceID = DeviceID;
                oFeedBack.UserName = UserName;
                oFeedBack.Email = Email;
                oFeedBack.Comment = Comment;
                oFeedBack.CreatedOn = DateTime.Now;
                oOFFERDBEntities.OfferFeedbacks.Add(oFeedBack);
                oOFFERDBEntities.SaveChanges();

                string body = string.Empty;
                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Content/EmailTemplate/UserFeedbackEmailBody.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{UserName}", UserName);
                body = body.Replace("{Email}", Email);
                body = body.Replace("{Feedback}", Comment);

                var adminMailList = oOFFERDBEntities.SysUsers.Where(t => t.IsActive == true && t.SysEnum.EnumName.ToLower().Contains("admin") && !string.IsNullOrEmpty(t.Email)).Select(x => x.Email).ToList();

                SendEmail.SendMail("Discount Buzz App Feedback", adminMailList, body);

                oPostItem.IsPostedSuccess = true;
                oPostItem.PostMsg = "Data Saved Successfully";

            }
            catch (Exception ex)
            {
                oPostItem.IsPostedSuccess = false;
                oPostItem.PostMsg = "Data Saved Unsuccessful due to " + ex.Message;
            }
            return oPostItem;
        }



        [Route("reviewOffer/{DeviceID}/{ReviewerName}/{OfferID}/{Rating}/{Comment}")]
        [HttpGet]
        public OfferReviewPostModel reviewOffer(string DeviceID, string ReviewerName, long OfferID, int Rating, string Comment)
        {
            OfferReviewPostModel oDbItem = new OfferReviewPostModel();
            oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            try
            {
                var oOfferReviewItem = oOFFERDBEntities.OfferReviews.FirstOrDefault(t => t.DeviceID == DeviceID && t.OfferID == OfferID);
                if (oOfferReviewItem != null)
                {
                    oOfferReviewItem.OfferReview1 = Rating;
                    oOfferReviewItem.OfferReviewerName = ReviewerName;
                    oOfferReviewItem.OfferReviewComment = Comment;
                    oOfferReviewItem.ModifiedOn = DateTime.Now;
                    oOFFERDBEntities.SaveChanges();

                    oDbItem.IsPostedSuccess = true;
                    oDbItem.OfferReviewDetails = oOfferReviewItem;
                }
                else
                {

                    OfferReview oReviewPost = new OfferReview();
                    oReviewPost.DeviceID = DeviceID;
                    oReviewPost.OfferID = OfferID;
                    oReviewPost.OfferReviewerName = ReviewerName;
                    oReviewPost.OfferReview1 = Rating;
                    oReviewPost.OfferReviewComment = Comment;
                    oReviewPost.CreatedOn = DateTime.Now;

                    oOFFERDBEntities.OfferReviews.Add(oReviewPost);
                    oOFFERDBEntities.SaveChanges();

                    oDbItem.IsPostedSuccess = true;
                    oDbItem.OfferReviewDetails = oReviewPost;
                }

            }
            catch (Exception ex)
            {
                oDbItem.IsPostedSuccess = false;
                oDbItem.OfferReviewDetails = null;
            }
            return oDbItem;
        }



        [Route("likeOffer/{DeviceID}/{OfferID}")]
        [HttpGet]
        public OfferReviewPostModel likeOffer(string DeviceID, long OfferID)
        {
            OfferReviewPostModel oDbItem = new OfferReviewPostModel();
            oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            try
            {
                var oOfferReviewItem = oOFFERDBEntities.OfferReviews.FirstOrDefault(t => t.DeviceID == DeviceID && t.OfferID == OfferID);
                if (oOfferReviewItem != null)
                {
                    oOfferReviewItem.OfferIsFavorite = true;
                    oOfferReviewItem.ModifiedOn = DateTime.Now;
                    oOFFERDBEntities.SaveChanges();

                    oDbItem.IsPostedSuccess = true;
                    oDbItem.OfferReviewDetails = oOfferReviewItem;
                }
                else
                {
                    OfferReview oReviewPost = new OfferReview();
                    oReviewPost.DeviceID = DeviceID;
                    oReviewPost.OfferReviewerName = "Anonymous";
                    oReviewPost.OfferID = OfferID;
                    oReviewPost.OfferIsFavorite = true;
                    oReviewPost.CreatedOn = DateTime.Now;

                    oOFFERDBEntities.OfferReviews.Add(oReviewPost);
                    oOFFERDBEntities.SaveChanges();

                    oDbItem.IsPostedSuccess = true;
                    oDbItem.OfferReviewDetails = oReviewPost;
                }

            }
            catch (Exception ex)
            {
                oDbItem.IsPostedSuccess = false;
                oDbItem.OfferReviewDetails = null;
            }
            return oDbItem;
        }


        //[Route("disLikeOffer/{DeviceID}/{OfferID}")]
        //[HttpGet]
        //public OfferReviewPostModel disLikeOffer(string DeviceID, long OfferID)
        //{
        //    OfferReviewPostModel oDbItem = new OfferReviewPostModel();
        //    oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
        //    try
        //    {
        //        var oOfferReviewItem = oOFFERDBEntities.OfferReviews.FirstOrDefault(t => t.DeviceID == DeviceID && t.OfferID == OfferID);
        //        if (oOfferReviewItem != null)
        //        {
        //            oOfferReviewItem.OfferIsFavorite = false;
        //            oOfferReviewItem.ModifiedOn = DateTime.Now;
        //            oOFFERDBEntities.SaveChanges();

        //            oDbItem.IsPostedSuccess = true;
        //            oDbItem.OfferReviewDetails = oOfferReviewItem;
        //        }
        //        else
        //        {
        //            oDbItem.IsPostedSuccess = false;
        //            oDbItem.OfferReviewDetails = null;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        oDbItem.IsPostedSuccess = false;
        //        oDbItem.OfferReviewDetails = null;
        //    }
        //    return oDbItem;
        //}


        [Route("unfollowOffer/{DeviceID}/{OfferID}")]
        [HttpGet]
        public OfferReviewPostModel unfollowOffer(string DeviceID, long OfferID)
        {
            OfferReviewPostModel oDbItem = new OfferReviewPostModel();
            oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            try
            {
                var oOfferReviewItem = oOFFERDBEntities.OfferReviews.FirstOrDefault(t => t.DeviceID == DeviceID && t.OfferID == OfferID);
                if (oOfferReviewItem != null)
                {
                    oOfferReviewItem.OfferIsFollow = false;
                    oOfferReviewItem.ModifiedOn = DateTime.Now;
                    oOFFERDBEntities.SaveChanges();

                    oDbItem.IsPostedSuccess = true;
                    oDbItem.OfferReviewDetails = oOfferReviewItem;
                }
                else
                {
                    OfferReview oReviewPost = new OfferReview();
                    oReviewPost.DeviceID = DeviceID;
                    oReviewPost.OfferReviewerName = "Anonymous";
                    oReviewPost.OfferID = OfferID;
                    oReviewPost.OfferIsFollow = false;
                    oReviewPost.CreatedOn = DateTime.Now;

                    oOFFERDBEntities.OfferReviews.Add(oReviewPost);
                    oOFFERDBEntities.SaveChanges();

                    oDbItem.IsPostedSuccess = true;
                    oDbItem.OfferReviewDetails = oReviewPost;
                }

            }
            catch (Exception ex)
            {
                oDbItem.IsPostedSuccess = false;
                oDbItem.OfferReviewDetails = null;
            }
            return oDbItem;
        }


        [Route("followOffer/{DeviceID}/{OfferID}")]
        [HttpGet]
        public OfferReviewPostModel followOffer(string DeviceID, long OfferID)
        {
            OfferReviewPostModel oDbItem = new OfferReviewPostModel();
            oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            try
            {
                var oOfferReviewItem = oOFFERDBEntities.OfferReviews.FirstOrDefault(t => t.DeviceID == DeviceID && t.OfferID == OfferID);
                if (oOfferReviewItem != null)
                {
                    oOfferReviewItem.OfferIsFollow = true;
                    oOfferReviewItem.ModifiedOn = DateTime.Now;
                    oOFFERDBEntities.SaveChanges();

                    oDbItem.IsPostedSuccess = true;
                    oDbItem.OfferReviewDetails = oOfferReviewItem;
                }
                else
                {
                    OfferReview oReviewPost = new OfferReview();
                    oReviewPost.DeviceID = DeviceID;
                    oReviewPost.OfferID = OfferID;
                    oReviewPost.OfferReviewerName = "Anonymous";
                    oReviewPost.OfferIsFollow = true;
                    oReviewPost.CreatedOn = DateTime.Now;

                    oOFFERDBEntities.OfferReviews.Add(oReviewPost);
                    oOFFERDBEntities.SaveChanges();

                    oDbItem.IsPostedSuccess = true;
                    oDbItem.OfferReviewDetails = oReviewPost;
                }

            }
            catch (Exception ex)
            {
                oDbItem.IsPostedSuccess = false;
                oDbItem.OfferReviewDetails = null;
            }
            return oDbItem;
        }


        #endregion

        #region Review
        [Route("getReviewList/{OfferID}")]
        [HttpGet]
        public List<OfferReview> getReviewList(long OfferID)
        {
            List<OfferReview> oDbItem = new List<OfferReview>();
            oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            try
            {
                oDbItem = oOFFERDBEntities.OfferReviews.Where(t => t.OfferID == OfferID).ToList();
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }

        #endregion

        #region OutletList
        [Route("getOutletList")]
        [HttpGet]
        public List<OfferOutletModel> getOutletList()
        {
            List<OfferOutletModel> oDbItem = new List<OfferOutletModel>();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;
            //oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            try
            {
                int featureType = Convert.ToInt32(Enumaretion.DBEnumType.OfferFeature);
                long enumhighPriorityId = oOFFERDBEntities.SysEnums.FirstOrDefault(t => t.EnumType == featureType && t.EnumName.ToLower().Contains("high")).ID;
                var oOfferOutlet = oOFFERDBEntities.OfferLocOutletMaps.Where(t => t.IsActive).ToList();
                OfferOutletModel oItem;
                foreach (OfferLocOutletMap oModel in oOfferOutlet)
                {
                    oItem = new OfferOutletModel();
                    oItem.OfferID = 0;
                    oItem.OfferOutletD = oModel.ID;
                    oItem.OfferOutletAddress = oModel.OutletAddress;
                    oItem.OfferOutletName = oModel.OutletName;
                    oItem.OfferOutletLocation = oModel.OfferLocation.LocationName;
                    oDbItem.Add(oItem);
                }
            }
            catch (Exception ex)
            {
                oDbItem = null;
            }
            return oDbItem;
        }

        #endregion

        #region Others
        [Route("getFollowCompanyUnseenOfferCount/{DeviceId}")]
        public FollowedCompanyUnseenOfferCount getFollowCompanyUnseenOfferCount(String DeviceId)
        {
            FollowedCompanyUnseenOfferCount unseenOfferCount= new FollowedCompanyUnseenOfferCount();
            oOFFERDBEntities.Configuration.LazyLoadingEnabled = true;
            //oOFFERDBEntities.Configuration.ProxyCreationEnabled = false;
            try
            {
                
                var oFollowedCompanyOffer = oOFFERDBEntities.OfferReviews.Where(t => t.OfferIsFollow == true && t.DeviceID == DeviceId).Select(x => x.OffersInfo.SysUser.ID).Distinct().ToList();
                var oOfferItem = oOFFERDBEntities.OffersInfoes.Where(t => t.IsActive == true && oFollowedCompanyOffer.Contains(t.SysUser.ID)).Count();

                var oFollowedCompanyOfferView = oOFFERDBEntities.OfferViews.Where(t => t.DeviceID == DeviceId && oFollowedCompanyOffer.Contains(t.OffersInfo.SysUser.ID) && t.OffersInfo.IsActive).Select(x => x.OfferID).Distinct().ToList().Count();

                long count = oOfferItem - oFollowedCompanyOfferView;
                unseenOfferCount.IsCountSuccessful = true;
                unseenOfferCount.unseenOfferCount = count;
            }
            catch (Exception ex)
            {
                unseenOfferCount.IsCountSuccessful = false;
                unseenOfferCount.unseenOfferCount = 0;
            }
            return unseenOfferCount;
        }
        #endregion
    }
}
