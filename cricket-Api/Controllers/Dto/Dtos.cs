namespace cricket_Api.Controllers.Dto
{
    public class LiveHomePageMatchOutputDto
    {
        public string? Title { get; set; } = string.Empty;
        public string? Url { get; set; } = string.Empty;
        public string? TeamOne { get; set; } = string.Empty;
        public string? TeamOneScore { get; set; } = string.Empty;
        public string? TeamTwo { get; set; } = string.Empty;
        public string? TeamTwoScore { get; set; } = string.Empty;
        public string? Results { get; set; } = string.Empty;
        public string? DateTime { get; set; } = string.Empty;

    }
    public class LiveHomePageMatchDetailOutputDto
    {
        public string? TourName { get; set; } = string.Empty;
        public string? TourVenue { get; set; } = string.Empty;
        public string? TourTitle { get; set; } = string.Empty;
        public string? MatchStatus { get; set; } = string.Empty;
        public List<Inning>? Innings { get; set; } = new List<Inning>();

    }

    public class Inning
    {
        public string? InningsName { get; set; } = string.Empty;
        public string? InningsStatus { get; set; } = string.Empty;
        public List<BatterDetail>? BatterDetails { get; set; } = new List<BatterDetail>();
        public List<BatterExtra>? BatterExtras { get; set; } = new List<BatterExtra>();
        public List<BowlerDetail>? BowlerDetails { get; set; } = new List<BowlerDetail>();

    }

    public class BatterDetail
    {
        public string? Title { get; set; } = string.Empty;
        public string? BatterName { get; set; } = string.Empty;
        public string? BattingStatus { get; set; } = string.Empty;
        public string? BatterRuns { get; set; } = string.Empty;
        public string? BatterBalls { get; set; } = string.Empty;
        public string? BatterFours { get; set; } = string.Empty;
        public string? BatterSixs { get; set; } = string.Empty;
        public string? BatterStrikeRate { get; set; } = string.Empty;
    }
    public class BowlerDetail
    {
        public string? BowlerName { get; set; } = string.Empty;
        public string? BowlergOvers { get; set; } = string.Empty;
        public string? BowlerMaiden { get; set; } = string.Empty;
        public string? BowlerRuns { get; set; } = string.Empty;
        public string? BowlerWicket { get; set; } = string.Empty;
        public string? BowlerNoBall { get; set; } = string.Empty;
        public string? BowlerWide { get; set; } = string.Empty;
        public string? BowlerEco { get; set; } = string.Empty;
    }

    public class BatterExtra
    {
        public string? TipOne { get; set; } = string.Empty;
        public string? TipTwo { get; set; } = string.Empty;
        public string? TipThree { get; set; } = string.Empty;
    }
}
