using System.Collections.Generic;
using PinterestTorBot.PinterestClient.Api.Enums;

namespace PinterestTorBot.PinterestClient.Api
{
    /// <summary>
    ///     Place of every endpoints, headers and other constants and variables.
    /// </summary>
    internal static class PinterestApiConstants
    {
        #region Main

        public const string UrlBase = "https://www.pinterest.com/";
        public const string DeveloperUrlBase = "https://developers.pinterest.com/";
        public const string DeveloperUrlApps = "https://developers.pinterest.com/apps/";
        public const string OauthUrlBase = "https://api.pinterest.com/oauth/";
        public const string OauthAccessUrlBase = "https://api.pinterest.com/v1/oauth/token";

        public const string Host = "www.pinterest.com";

        public const string CsrfCookieField = "csrftoken";
        public const string SessionCookieField = "_pinterest_sess";

        public const string AppVersionHeaderField = "pinterest-version";

        public const string HeaderUserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36";

        public const string HeaderXAppVersion = "93a0597";

        public const int DefaultPageSize = 25;

        #endregion Main

        #region Login

        public const string ResourceLogin = "resource/UserSessionResource/create/";

        #endregion

        #region Password

        public const string ResourceResetPasswordSendLink = "resource/UserResetPasswordResource/create/";
        public const string ResourceResetPasswordUpdate = "resource/ResetPasswordFromEmailResource/update/";

        #endregion

        #region Boards

        public const string ResourceGetBoards = "resource/BoardsResource/get/";
        public const string ResourceGetBoard = "resource/BoardResource/get/";
        public const string ResourceGetBoardFeed = "resource/BoardFeedResource/get/";
        public const string ResourceProfileBoards = "resource/ProfileBoardsResource/get/";
        public const string ResourceFollowBoard = "resource/BoardFollowResource/create/";
        public const string ResourceUnfollowBoard = "resource/BoardFollowResource/delete/";
        public const string ResourceDeleteBoard = "resource/BoardResource/delete/";
        public const string ResourceArchiveBoard = "resource/BoardArchiveResource/update/";
        public const string ResourceCreateBoard = "resource/BoardResource/create/";
        public const string ResourceUpdateBoard = "resource/BoardResource/update/";
        public const string ResourceBoardFollowers = "resource/BoardFollowersResource/get/";
        public const string ResourceFollowingBoards = "resource/BoardFollowingResource/get/";
        public const string ResourceTitleSuggestions = "resource/BoardTitleSuggestionsResource/get";
        public const string ResourceBoardsInvites = "resource/BoardInvitesResource/get/";
        public const string ResourceCreateUserIdInvite = "resource/BoardInviteResource/create";
        public const string ResourceCreateEmailInvite = "resource/BoardEmailInviteResource/create/";
        public const string ResourceAcceptInvite = "resource/BoardInviteResource/update/";
        public const string ResourceDeleteInvite = "resource/BoardInviteResource/delete/";
        public const string ResourceLeaveBoard = "resource/BoardCollaboratorResource/delete/";
        public const string ResourceGetBoardsLookup = "resource/BoardPickerBoardsResource/get/";

        #endregion

        #region BoardSection

        public const string ResourceGetBoardSections = "resource/BoardSectionsResource/get/";
        public const string ResourceAddBoardSection = "resource/BoardSectionResource/create/";
        public const string ResourceEditBoardSection = "resource/BoardSectionEditResource/create/";
        public const string ResourceDeleteBoardSection = "resource/BoardSectionEditResource/delete/";
        public const string ResourceGetBoardSectionPins = "resource/BoardSectionPinsResource/get/";
        public const string ResourceGetBoardSectionsLookup = "resource/BoardSectionsRepinResource/get/";
        public const string ResourceBulkMoveBoardSectionPins = "resource/BoardSectionEditResource/update/";
        public const string ResourceBulkMoveBoardSectionPinsToBoard = "resource/BoardSectionPinsResource/delete/";

        #endregion

        #region Pins

        public const string ResourceCreatePin = "resource/PinResource/create/";
        public const string ResourceUpdatePin = "resource/PinResource/update/";
        public const string ResourceRepin = "resource/RepinResource/create/";
        public const string ResourceUserFollowers = "resource/UserFollowersResource/get/";
        public const string ResourceDeletePin = "resource/PinResource/delete/";
        public const string ResourceCommentLike = "resource/AggregatedCommentLikeResource/create/";
        public const string ResourceCommentUnlike = "resource/AggregatedCommentLikeResource/delete/";
        public const string ResourceCommentFeed = "resource/AggregatedCommentFeedResource/get/";
        public const string ResourceCommentPin = "resource/AggregatedCommentResource/create/";
        public const string ResourceCommentDeletePin = "resource/AggregatedCommentResource/delete/";
        public const string ResourcePinPage = "resource/PinPageResource/get/";
        public const string ResourcePinInfo = "resource/PinResource/get/";
        public const string ResourceDomainFeed = "resource/DomainFeedResource/get/";
        public const string ResourceActivity = "resource/AggregatedActivityFeedResource/get/";
        public const string ResourceUserFeed = "resource/UserHomefeedResource/get/";
        public const string ResourceRelatedPins = "resource/RelatedPinFeedResource/get/";
        public const string ResourceVisualSimilarPins = "resource/VisualLiveSearchResource/get/";
        public const string ResourceBulkCopy = "resource/BulkEditResource/create/";
        public const string ResourceBulkMove = "resource/BulkEditResource/update/";
        public const string ResourceBulkDelete = "resource/BulkEditResource/delete/";
        public const string ResourceExplorePins = "resource/ExploreSectionFeedResource/get/";
        public const string ResourcePinAnalytics = "resource/OnPinAnalyticsResource/get/";
        public const string ResourceTryPinCreate = "resource/DidItActivityResource/create/";
        public const string ResourceTryPinEdit = "resource/DidItActivityResource/update/";
        public const string ResourceTryPinDelete = "resource/DidItActivityResource/delete/";
        public const string ResourceTryPinImageUpload = "resource/DidItImageUploadResource/create/";
        public const string ResourceShareViaSocial = "resource/CreateExternalInviteResource/create/";
        public const string ResourceOffsiteLink = "resource/OffsiteLinkResource/get/";

        #endregion

        #region Pinners

        public const string ResourceFollowUser = "resource/UserFollowResource/create/";
        public const string ResourceUnfollowUser = "resource/UserFollowResource/delete/";
        public const string ResourceUserInfo = "resource/UserResource/get/";
        public const string ResourceUserFollowing = "resource/UserFollowingResource/get/";
        public const string ResourceUserPins = "resource/UserPinsResource/get/";
        public const string ResourceUserLikes = "resource/UserLikesResource/get/";
        public const string ResourceUserTried = "resource/DidItUserFeedResource/get/";
        public const string ResourceContactsRequests = "resource/ContactRequestsResource/get";
        public const string ResourceContactRequestAccept = "resource/ContactRequestAcceptResource/update/";
        public const string ResourceContactRequestIgnore = "resource/ContactRequestIgnoreResource/delete/";

        #endregion

        #region Search

        public const string ResourceSearch = "resource/BaseSearchResource/get/";
        public const string ResourceSearchWithPagination = "resource/SearchResource/get/";
        public const string ResourceTypeAheadSuggestions = "resource/AdvancedTypeaheadResource/get";
        public const string ResourceHashtagTypeAheadSuggestions = "resource/HashtagTypeaheadResource/get";

        #endregion

        #region Interests

        public const string ResourceFollowInterest = "resource/InterestFollowResource/create/";
        public const string ResourceUnfollowInterest = "resource/InterestFollowResource/delete/";
        public const string ResourceFollowingInterests = "resource/InterestFollowingResource/get/";

        #endregion

        #region Conversations

        public const string ResourceSendMessage = "resource/ConversationsResource/create/";
        public const string ResourceGetConversationMessages = "resource/ConversationMessagesResource/get/";
        public const string ResourceGetLastConversations = "resource/ConversationsResource/get/";

        #endregion

        #region UserSettings

        public const string ResourceUpdateUserSettings = "resource/UserSettingsResource/update/";
        public const string ResourceGetUserSettings = "resource/UserSettingsResource/get/";
        public const string ResourceChangePassword = "resource/UserPasswordResource/update/";
        public const string ResourceDeactivateAccount = "resource/DeactivateAccountResource/create/";
        public const string ResourceBlockUser = "resource/UserBlockResource/create/";
        public const string ResourceUnBlockUser = "resource/UserBlockResource/delete/";
        public const string ResourceClearSearchHistory = "resource/TypeaheadClearRecentResource/delete/";
        public const string ResourceSessionsHistory = "resource/UserSessionStoreResource/get/";
        public const string ResourceAvailableLocales = "resource/LocalesResource/get/";
        public const string ResourceAvailableCountries = "resource/CountriesResource/get/";
        public const string ResourceAvailableAccountTypes = "resource/BusinessTypesResource/get/";

        #endregion

        #region News

        public const string ResourceGetLatestNews = "resource/NetworkStoriesResource/get/";
        public const string ResourceGetNotifications = "resource/NewsHubResource/get/";
        public const string ResourceGetNewsHubDetails = "resource/NewsHubDetailsResource/get/";

        #endregion

        #region Registration

        public const string ResourceCreateRegister = "resource/UserRegisterResource/create/";
        public const string ResourceCheckEmail = "resource/EmailExistsResource/get/";
        public const string ResourceSetOrientation = "resource/OrientationContextResource/create/";
        public const string ResourceUpdateRegistrationTrack = "resource/UserRegisterTrackActionResource/update/";
        public const string ResourceRegistrationComplete = "resource/UserExperienceCompletedResource/update/";
        public const string ResourceConvertToBusiness = "resource/PartnerResource/update/";
        public const string ResourceUserState = "resource/UserStateResource/create/";
        public const string ResourceUserStateGet = "resource/UserStateResource/get/";

        #endregion

        #region Uploads

        public const string ImageUpload = "upload-image/";

        #endregion

        #region Categories

        public const string ResourceGetCategories = "resource/CategoriesResource/get/";
        public const string ResourceGetCategory = "resource/CategoryResource/get/";
        public const string ResourceGetCategoriesRelated = "resource/RelatedInterestsResource/get/";
        public const string ResourceGetCategoryFeed = "resource/CategoryFeedResource/get/";

        #endregion

        #region Topics

        public const string ResourceGetTopicFeed = "resource/TopicFeedResource/get/";
        public const string ResourceGetTopic = "resource/TopicResource/get/";
        public const string ResourceExploreSections = "resource/ExploreSectionsResource/get/";

        #endregion

        #region Invite

        public const string ResourceInvite = "resource/EmailInviteSentResource/create/";

        #endregion

        #region Following

        public static readonly IDictionary<FollowingType, string> FollowingUrls =
            new Dictionary<FollowingType, string>()
            {
                {FollowingType.Boards, ResourceFollowingBoards},
                {FollowingType.Interests, ResourceFollowingInterests},
                {FollowingType.People, ResourceUserFollowing},
            };

        #endregion
    }
}