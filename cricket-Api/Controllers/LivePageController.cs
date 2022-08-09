using cricket_Api.Controllers.Dto;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace cricket_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LivePageController : ControllerBase
    {
        public LivePageController()
        {

        }

        [HttpGet("/getlivescore")]
        public async Task<List<LiveHomePageMatchOutputDto>> GetAsync()
        {
            var url = "https://www.cricbuzz.com/";
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);
            var node = doc.DocumentNode.SelectSingleNode("//div[contains(@id,'match_menu_container')]");
            var matchNodes = node.SelectSingleNode("//ul[contains(@class,'videos-carousal-wrapper')]").ChildNodes.Where(c => c.InnerHtml.Trim() != "").ToList();
            List<LiveHomePageMatchOutputDto> matchOutput = new List<LiveHomePageMatchOutputDto>();
            foreach (var item in matchNodes)
            {
                LiveHomePageMatchOutputDto liveHome = new LiveHomePageMatchOutputDto();
                var atag = item.ChildNodes.Where(c => c.Name == "a").FirstOrDefault();
                liveHome.Title = atag?.Attributes["title"].Value;
                liveHome.Url = atag?.Attributes["href"].Value;
                liveHome.Url = liveHome.Url?.Replace("-scores", "-scorecard");

                var nextDiv = item.SelectSingleNode(".//div[contains(@class,'cb-pos-rel')]");
                var dataDivs = nextDiv.SelectNodes("./div/div");
                liveHome.TeamOne = dataDivs[0]?.SelectNodes("./div")?.FirstOrDefault()?.InnerHtml;
                liveHome.TeamOneScore = dataDivs[0]?.SelectNodes("./div")?.LastOrDefault()?.InnerHtml;
                if (string.IsNullOrEmpty(liveHome.TeamOne))
                {
                    liveHome.TeamOne = dataDivs[0]?.InnerHtml;
                }
                liveHome.TeamTwo = dataDivs[1]?.SelectNodes("./div")?.FirstOrDefault()?.InnerHtml;
                liveHome.TeamTwoScore = dataDivs[1]?.SelectNodes("./div")?.LastOrDefault()?.InnerHtml;
                if (string.IsNullOrEmpty(liveHome.TeamTwo))
                {
                    liveHome.TeamTwo = dataDivs[1]?.InnerHtml;
                }
                liveHome.Results = dataDivs[2]?.InnerHtml;
                if (string.IsNullOrEmpty(liveHome.Results))
                {
                    liveHome.DateTime = dataDivs[3]?.Attributes["ng-bind"]?.Value;
                    if (!string.IsNullOrEmpty(liveHome.DateTime))
                    {
                        liveHome.DateTime = Regex.Match(liveHome.DateTime, @"\d+").Value;

                    }
                }

                matchOutput.Add(liveHome);

            }
            return matchOutput;
        }

        [HttpPost("/getlivescoredetail")]
        public async Task<LiveHomePageMatchDetailOutputDto> GetDetailAsync([FromBody] string nextPageUrl)
        {
            var url = string.Concat("https://www.cricbuzz.com/", nextPageUrl);
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);
            LiveHomePageMatchDetailOutputDto matchOutput = new LiveHomePageMatchDetailOutputDto();
            var nodes = doc.DocumentNode.SelectSingleNode("//div[contains(@id,'page-wrapper')]").ChildNodes;
            var dataStreamNode = nodes.Where(c => c.Name == "div" && c.Attributes["class"].Value.Equals("cb-col cb-col-100 cb-bg-white")).FirstOrDefault();
            var dataStreamChildNodes = dataStreamNode.ChildNodes.Where(c => c.Name == "div").ToList();
            var headerNode = dataStreamChildNodes[0];
            var bodyNode = dataStreamChildNodes[1];
            //header Details
            var tourDetail = headerNode.ChildNodes.Where(c => c.Name == "div").LastOrDefault()?.ChildNodes.Where(c => c.Name == "a").ToList();
            var tourName = tourDetail[0].InnerText;
            var tourVenue = tourDetail[1].InnerText;
            var TourTitle = dataStreamNode.SelectSingleNode(".//h1").InnerText;
            var Matchstatus = bodyNode.SelectSingleNode(".//div[contains(@class,'cb-scrcrd-status')]").InnerText;
            matchOutput.TourName = tourName;
            matchOutput.TourVenue = tourVenue.Replace("&nbsp;", " ");
            matchOutput.TourTitle = TourTitle;
            string StatusWithoutSpace = Matchstatus.Replace("&nbsp;", " ");
            matchOutput.MatchStatus = StatusWithoutSpace;
            //header end

            //Body details
            for (int i = 1; i < 4; i++)
            {
                var OutputInnigs = new Inning();
                OutputInnigs.InningsName = string.Concat("Innings ", i);
                var Innings = bodyNode.SelectSingleNode(".//div[contains(@id,'innings_" + i + "')]");
                //  var inningsTwo = bodyNode.SelectSingleNode(".//div[contains(@id,'innings_2')]");
                var InningsBatterNodes = Innings?.ChildNodes?.Where(c => c.Name == "div")?.FirstOrDefault()?.SelectNodes(".//div[contains(@class,'cb-scrd-itms')]")?.ToList();
                var InnningsOneStatus = Innings?.ChildNodes?.Where(c => c.Name == "div")?.FirstOrDefault()?.ChildNodes?.Where(c => c.Name == "div")?.FirstOrDefault()?.InnerText;
                OutputInnigs.InningsStatus = InnningsOneStatus;
                List<BatterDetail>? BatterDetails = new List<BatterDetail>();
                List<BatterExtra>? BatterExtras = new List<BatterExtra>();
                if (InningsBatterNodes != null)
                    foreach (var batter in InningsBatterNodes)
                    {
                        var endBatterNodes = batter.ChildNodes.Where(c => c.Name == "div").ToList();
                        if (endBatterNodes.Count() > 5)
                        {
                            BatterDetail batterDetail = new BatterDetail();
                           batterDetail. BatterName = endBatterNodes[0].InnerText;
                           batterDetail. BattingStatus = endBatterNodes[1].InnerText;
                           batterDetail. BatterRuns = endBatterNodes[2].InnerText;
                           batterDetail. BatterBalls = endBatterNodes[3].InnerText;
                           batterDetail. BatterFours = endBatterNodes[4].InnerText;
                           batterDetail. BatterSixs = endBatterNodes[5].InnerText;
                           batterDetail. BatterStrikeRate = endBatterNodes[6].InnerText;
                            BatterDetails.Add(batterDetail);
                        }
                        else
                        {
                            BatterExtra batterExtra = new BatterExtra();
                            batterExtra. TipOne = endBatterNodes[0].InnerText;
                            batterExtra. TipTwo = endBatterNodes[1].InnerText.Replace("&nbsp;", " ");
                            batterExtra. TipThree = batterExtra.TipOne != " Did not Bat " && batterExtra.TipOne != " Yet to Bat " ? endBatterNodes[2]?.InnerText.Replace("&nbsp;", " ") : "";
                            BatterExtras.Add(batterExtra);
                        }
                    }
                OutputInnigs.BatterDetails = BatterDetails;
                OutputInnigs.BatterExtras = BatterExtras;
                var InningsBowlerNodes = Innings?.ChildNodes?.Where(c => c.Name == "div")?.Take(4).LastOrDefault()?.SelectNodes(".//div[contains(@class,'cb-scrd-itms')]")?.ToList();
                if (InningsBowlerNodes==null)
                {
                     InningsBowlerNodes = Innings?.ChildNodes?.Where(c => c.Name == "div")?.Take(2).LastOrDefault()?.SelectNodes(".//div[contains(@class,'cb-scrd-itms')]")?.ToList();

                }
                List<BowlerDetail>? BowlerDetails =new List<BowlerDetail>();
                if (InningsBowlerNodes != null)
                    foreach (var bowler in InningsBowlerNodes)
                    {
                        var endBowlerNodes = bowler.ChildNodes.Where(c => c.Name == "div").ToList();
                        if (endBowlerNodes.Count() > 7)
                        {
                            BowlerDetail bowlerDetail = new BowlerDetail();
                            bowlerDetail.BowlerName = endBowlerNodes[0].InnerText;
                            bowlerDetail.BowlergOvers = endBowlerNodes[1].InnerText;
                            bowlerDetail.BowlerMaiden = endBowlerNodes[2].InnerText;
                            bowlerDetail.BowlerRuns = endBowlerNodes[3].InnerText;
                            bowlerDetail.BowlerWicket = endBowlerNodes[4].InnerText;
                            bowlerDetail.BowlerNoBall = endBowlerNodes[5].InnerText;
                            bowlerDetail.BowlerWide = endBowlerNodes[6].InnerText;
                            bowlerDetail.BowlerEco = endBowlerNodes[7].InnerText;
                            BowlerDetails.Add(bowlerDetail);
                        }

                    }
                OutputInnigs.BowlerDetails = BowlerDetails;
                if ((OutputInnigs.BatterDetails!=null && OutputInnigs.BatterDetails.Count()>0 )|| (OutputInnigs.BowlerDetails!=null&&  OutputInnigs.BowlerDetails.Count() > 0))
                {

                    matchOutput.Innings?.Add(OutputInnigs);
                }
            }

            //Body details end

            return matchOutput;
        }
    }


}
