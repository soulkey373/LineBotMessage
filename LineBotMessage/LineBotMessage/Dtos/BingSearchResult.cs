namespace LineBotMessage.Dtos
{
    class ImageResult
    {
        public string? webSearchUrl { get; set; }
        public string? name { get; set; }
        public string? thumbnailUrl { get; set; }
        public DateTime? datePublished { get; set; }
        public bool? isFamilyFriendly { get; set; }
        public string? contentUrl { get; set; }
        public string? hostPageUrl { get; set; }
        public string? contentSize { get; set; }
        public string? encodingFormat { get; set; }
        public string? hostPageDisplayUrl { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public string? hostPageFavIconUrl { get; set; }
        public DateTime? hostPageDiscoveredDate { get; set; }
        public Thumbnail? thumbnail { get; set; }
        public string? imageInsightsToken { get; set; }
        public InsightsMetadata? insightsMetadata { get; set; }
        public string? imageId { get; set; }
        public string? accentColor { get; set; }
    }

    class Thumbnail
    {
        public int? width { get; set; }
        public int? height { get; set; }
    }

    class InsightsMetadata
    {
        public int? pagesIncludingCount { get; set; }
        public int? availableSizesCount { get; set; }
    }
}