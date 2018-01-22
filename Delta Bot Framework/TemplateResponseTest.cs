using AdaptiveCards;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using SampleBotTemplate.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;

namespace SampleBotTemplate
{
    public class TemplateResponseTest
    {
        public static Attachment plAttachment = new Attachment();
        public static List<Attachment> plAttachmentList = new List<Attachment>();
        public static string status;
        public static string flightNo;
        public static string seatmapUrl = ConfigurationManager.AppSettings["seatmapUrl"];
        public static CustomerResponse GetHeroCard(CustomerResponse custResInfo, Cards card, string UserId)
        {
            List<CardImage> cardImages = new List<CardImage>();

            if (card.HeroImg != "")
            {
                cardImages.Add(new CardImage(url: card.HeroImg));
            }

            List<CardAction> cardButtons = new List<CardAction>();
            {
                for (int i = 0; i < card.buttonCount; i++)
                {
                    CardAction plButton = new CardAction()
                    {
                        Value = card.buttonValue[i],
                        Type = card.buttonType[i],
                        Title = card.buttonTitle[i]
                    };
                    cardButtons.Add(plButton);
                }

                HeroCard plCard = new HeroCard()
                {
                    Title = card.HeroTitle,
                    Subtitle = card.HeroSubtitle,
                    Text = card.HeroText,
                    Images = cardImages,
                    Buttons = cardButtons

                };

                custResInfo.plAttachment = plCard.ToAttachment();
            }
            return custResInfo;
        }
        public static CustomerResponse GetAdaptiveUpdate(CustomerResponse custResInfo, Customer custInfo, BookingStatus bookingStatus, FlightStatus fltStatus)
        {
            string d = bookingStatus.departureTime.Substring(0, 10);
            DateTime dt = Convert.ToDateTime(d);
            string newdate = dt.ToString("dddd, dd MMM yyyy");
            string time = bookingStatus.departureTime.Substring(11, 5);

            AdaptiveCard card = new AdaptiveCard();
            card.Body.Add(
           new ColumnSet()
           {
               Separation = SeparationStyle.Strong,
               Columns = new List<Column>()
               {
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new Image()
                            {
                                Url = "http://messagecardplayground.azurewebsites.net/assets/Airplane.png",
                                Size = ImageSize.Small
                            }
                        }
                    },
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = "Flight Status",
                                HorizontalAlignment = HorizontalAlignment.Right
                            },
                            new TextBlock()
                            {
                                Text = bookingStatus.flightStatus,
                                 HorizontalAlignment = HorizontalAlignment.Right,
                                 Color = TextColor.Warning
                            }
                        }
                    }
               }
           }
           );

            card.Body.Add(
                new TextBlock()
                {
                    Separation = SeparationStyle.Strong,
                    Text = "Passengers",
                    IsSubtle = false,
                    Weight = TextWeight.Bolder
                });
            card.Body.Add(
                new TextBlock()
                {
                    Text = custInfo.FirstName + "  " + custInfo.LastName
                }
                );
            card.Body.Add(
           new ColumnSet()
           {
               Separation = SeparationStyle.Strong,
               Columns = new List<Column>()
               {
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = "Flight",
                            },
                            new TextBlock()
                            {
                                Text = "DL"+bookingStatus.flightNumber
                            }
                        }
                    },
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = "Departs"
                            },
                            new TextBlock()
                            {
                               Text = bookingStatus.departureCity
                            }

                        }
                    },
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = "Arrives"

                            },
                            new TextBlock()
                            {
                                Text = bookingStatus.arrivalCity
                            }
                        }
                    }
               }
           }
           );
            card.Body.Add(
            new ColumnSet()
            {
                Separation = SeparationStyle.Strong,
                Columns = new List<Column>()
                {
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = bookingStatus.departureCity
                            },
                            new TextBlock()
                            {
                                Text = bookingStatus.departureStation,
                                Size = TextSize.ExtraLarge,
                                Color = TextColor.Accent
                            }
                        }
                    },
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = "\t\t"
                            },
                            new Image()
                            {
                                Url= "http://messagecardplayground.azurewebsites.net/assets/airplane.png",
                                Size = ImageSize.Small
                            }

                        }
                    },
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = bookingStatus.arrivalCity

                            },
                            new TextBlock()
                            {
                                Text = bookingStatus.departureStation,
                                Size = TextSize.ExtraLarge,
                                Color = TextColor.Accent,
                                HorizontalAlignment = HorizontalAlignment.Right
                            }
                        }
                    }
                }
            }
            );
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
            custResInfo.plAttachment = attachment;
            custResInfo.respTxt = null;
            return custResInfo;
        }
        public static CustomerResponse GetAdaptiveBoarding(CustomerResponse custResInfo, Customer custInfo, BookingStatus bookingStatus, FlightStatus fltStatus)
        {
            string d = custResInfo.departureTime.Substring(0, 10);
            DateTime dt = Convert.ToDateTime(d);
            string newdate = dt.ToString("dddd, dd MMM yyyy");
            string time = custResInfo.departureTime.Substring(11, 5);

            AdaptiveCard card = new AdaptiveCard();

            card.Body.Add(
                new Image()
                {
                    Url = "https://botstoragedl1.file.core.windows.net/movies/FWF_QR_code.jpg",
                    Size = ImageSize.Small
                }
                );
            card.Body.Add(
                new TextBlock()
                {
                    Separation = SeparationStyle.Strong,
                    Text = "Passengers",
                    IsSubtle = false,
                    Weight = TextWeight.Bolder
                });
            card.Body.Add(
                new TextBlock()
                {
                    Text = custInfo.FirstName + "  " + custInfo.LastName
                }
                );
            card.Body.Add(
                new TextBlock()
                {
                    Text = newdate + " at " + time,
                    Weight = TextWeight.Bolder,
                    Separation = SeparationStyle.Strong,
                });
            card.Body.Add(
           new ColumnSet()
           {
               Separation = SeparationStyle.Strong,
               Columns = new List<Column>()
               {
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = "Gate",
                            },
                            new TextBlock()
                            {
                                Text = fltStatus.departureGate,
                                Size = TextSize.Medium
                            }
                        }
                    },
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = "\t\t"
                            },
                            new TextBlock()
                            {
                               Text = "DL"+custResInfo.selectedFlightNo
                            }

                        }
                    },
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = "Departs"

                            },
                            new TextBlock()
                            {
                                Text = time
                            }
                        }
                    }
               }
           }
           );
            card.Body.Add(
            new ColumnSet()
            {
                Separation = SeparationStyle.Strong,
                Columns = new List<Column>()
                {
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = bookingStatus.departureCity
                            },
                            new TextBlock()
                            {
                                Text = bookingStatus.departureStation,
                                Size = TextSize.ExtraLarge,
                                Color = TextColor.Accent
                            }
                        }
                    },
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = "\t\t"
                            },
                            new Image()
                            {
                                Url= "http://messagecardplayground.azurewebsites.net/assets/airplane.png",
                                Size = ImageSize.Small
                            }

                        }
                    },
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = custResInfo.arrivalCity

                            },
                            new TextBlock()
                            {
                                Text = custResInfo.arrivalStation,
                                Size = TextSize.ExtraLarge,
                                Color = TextColor.Accent,
                                HorizontalAlignment = HorizontalAlignment.Right
                            }
                        }
                    }
                }
            }
            );
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
            custResInfo.plAttachment = attachment;
            custResInfo.respTxt = null;
            return custResInfo;
        }
        public static CustomerResponse GetAdaptiveItinerary(CustomerResponse custResInfo, Customer custInfo, BookingStatus bookingStatus, FlightStatus fltStatus)
        {
            string d = custResInfo.departureTime.Substring(0, 10);
            DateTime dt = Convert.ToDateTime(d);
            string newdate = dt.ToString("dddd, dd MMM yyyy");
            string time = custResInfo.departureTime.Substring(11, 5);

            AdaptiveCard card = new AdaptiveCard();
            card.Speak = "<s>Your flight is confirmed for you from " + bookingStatus.departureCity + " to " + custResInfo.arrivalCity + " on " + newdate + " at " + time + " </s>";

            // Add text to the card.
            card.Body.Add(new TextBlock()
            {
                Text = "Passengers",
                IsSubtle = false,
                Weight = TextWeight.Bolder
            });
            card.Body.Add(new TextBlock()
            {
                Text = custInfo.FirstName + "  " + custInfo.LastName
            });
            card.Body.Add(new TextBlock()
            {
                Text = newdate + " at " + time,
                Weight = TextWeight.Bolder,
                Separation = SeparationStyle.Strong,
            });
            card.Body.Add(
            new ColumnSet()
            {
                Separation = SeparationStyle.Strong,
                Columns = new List<Column>()
                {
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = bookingStatus.departureCity
                            },
                            new TextBlock()
                            {
                                Text = bookingStatus.departureStation,
                                Size = TextSize.ExtraLarge,
                                Color = TextColor.Accent
                            }
                        }
                    },
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = "\t\t"
                            },
                            new Image()
                            {
                                Url= "http://messagecardplayground.azurewebsites.net/assets/airplane.png",
                                Size = ImageSize.Small
                            }

                        }
                    },
                    new Column()
                    {
                        Items = new List<CardElement>()
                        {
                            new TextBlock()
                            {
                                Text = custResInfo.arrivalCity

                            },
                            new TextBlock()
                            {
                                Text = custResInfo.arrivalStation,
                                Size = TextSize.ExtraLarge,
                                Color = TextColor.Accent,
                                HorizontalAlignment = HorizontalAlignment.Right
                            }
                        }
                    }
                }
            }
            );
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
            custResInfo.plAttachment = attachment;
            custResInfo.respTxt = null;
            return custResInfo;
        }
        public static CustomerResponse GetCategoryCarouselCard(List<string> Categorys,Cards cards,CustomerResponse custResInfo,string UserId)
        {
            plAttachmentList.Clear();
            int count = Categorys.Count;
            for (int i = 1; i <= count; i++)
            {
                cards.buttonCount = 1;
                cards.buttonValue[0] = Categorys[i-1];
                cards.buttonType[0] = "imBack";
                cards.buttonTitle[0] = $"{Categorys[i-1]}";
                switch (Categorys[i - 1].ToLower())
                {
                    case "beach":
                        cards.HeroImg = "http://www.publicdomainpictures.net/pictures/150000/velka/tropical-beach-1454007190ZAK.jpg";
                        break;
                    case "city":
                        cards.HeroImg = "http://www.calgary.ca/CA/city-manager/scripts/about-us/webparts/images/ourHistory_retina.jpg";
                        break;
                    case "mountain":
                        cards.HeroImg = "https://onehdwallpaper.com/wp-content/uploads/2015/07/Free-Download-Mountain-Hd-Wallpapers.jpg";
                            break;
                }
                custResInfo = GetHeroCard(custResInfo, cards, UserId);
                plAttachmentList.Add(custResInfo.plAttachment);
            }
            custResInfo.plAttachment = null;
            custResInfo.plAttachmentList = plAttachmentList;
            return custResInfo;
        }
        public static CustomerResponse GetStationCarouselCard(List<ReverseQuery> results, Cards cards, CustomerResponse custResInfo, string UserId)
        {
            plAttachmentList.Clear();
            int count = results.Count;
            for (int i = 1; i <= count; i++)
            {
                cards.buttonCount = 1;
                cards.buttonValue[0] = results[i-1].Stations;
                cards.buttonType[0] = "imBack";
                cards.buttonTitle[0] = $"{results[i - 1].Stations}";
                cards.HeroTitle = results[i - 1].Type;
                cards.HeroSubtitle = results[i - 1].Price;
                switch (results[i - 1].Stations.ToLower())
                {
                    case "msp":
                        //cards.HeroImg = "https://blog.loewshotels.com/wp-content/uploads/2015/05/iStock_000010323760_Large.jpg";
                        cards.HeroImg = "http://cdn.minneapolis.org/CMS/39/minneapolis_skyline__hero.jpg";
                        break;
                    case "ord":
                        cards.HeroImg = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f2/Ord%2C_Nebraska_L_Street_1.JPG/1200px-Ord%2C_Nebraska_L_Street_1.JPG";
                        break;
                    case "mia":
                        cards.HeroImg = "https://i.pinimg.com/originals/99/92/33/9992336feb7d00a162b35014502656a9.jpg";
                        break;
                    case "sav":
                        cards.HeroImg = "http://www.saccity.org/uploads/3/0/8/4/30845173/7268124_orig.jpg";
                        break;
                    case "den":
                        cards.HeroImg = "https://i.ytimg.com/vi/FWs1b96c7wg/hqdefault.jpg";
                        break;
                    case "sea":
                        cards.HeroImg = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c0/Seaislecity-nj-usa.jpg/550px-Seaislecity-nj-usa.jpg";
                        break;
                    case "sfo":
                        cards.HeroImg = "https://images.trvl-media.com/hotels/1000000/690000/688000/687919/687919_36_z.jpg";
                        break;
                }
                custResInfo = GetHeroCard(custResInfo, cards, UserId);
                plAttachmentList.Add(custResInfo.plAttachment);
            }
            custResInfo.plAttachment = null;
            custResInfo.plAttachmentList = plAttachmentList;
            return custResInfo;
        }
        public static CustomerResponse GetFlightsCarouselCard(List<FlightTrip> results, Cards cards, CustomerResponse custResInfo, string UserId)
        {
            plAttachmentList.Clear();
            int count = results.Count;
            for (int i = 1; i <= count; i++)
            {
                cards.buttonCount = 1;
                cards.buttonValue[0] = results[i - 1].FlightNumber;
                cards.buttonType[0] = "imBack";
                cards.buttonTitle[0] = $"Flight - {results[i - 1].FlightNumber}";
                cards.HeroTitle = "Price - " + results[i - 1].Price;
                cards.HeroSubtitle = "Duration - " + results[i - 1].Duration;
                cards.HeroImg = "https://consumermediallc.files.wordpress.com/2015/05/deltapull.jpg";
                //switch (custResInfo.InputMessage.ToLower())
                //{
                //    case "msp":
                //        cards.HeroImg = "https://blog.loewshotels.com/wp-content/uploads/2015/05/iStock_000010323760_Large.jpg";
                //        break;
                //    case "ord":
                //        cards.HeroImg = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f2/Ord%2C_Nebraska_L_Street_1.JPG/1200px-Ord%2C_Nebraska_L_Street_1.JPG";
                //        break;
                //    case "mia":
                //        cards.HeroImg = "https://i.pinimg.com/originals/99/92/33/9992336feb7d00a162b35014502656a9.jpg";
                //        break;
                //    case "sav":
                //        cards.HeroImg = "http://www.saccity.org/uploads/3/0/8/4/30845173/7268124_orig.jpg";
                //        break;
                //    case "den":
                //        cards.HeroImg = "https://i.ytimg.com/vi/FWs1b96c7wg/hqdefault.jpg";
                //        break;
                //    case "sea":
                //        cards.HeroImg = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c0/Seaislecity-nj-usa.jpg/550px-Seaislecity-nj-usa.jpg";
                //        break;
                //    case "sfo":
                //        cards.HeroImg = "https://images.trvl-media.com/hotels/1000000/690000/688000/687919/687919_36_z.jpg";
                //        break;
                //}
                custResInfo = GetHeroCard(custResInfo, cards, UserId);
                plAttachmentList.Add(custResInfo.plAttachment);
            }
            custResInfo.plAttachment = null;
            custResInfo.plAttachmentList = plAttachmentList;
            return custResInfo;
        }
        public static CustomerResponse GetCarouselCard(Cards cards, BookingStatus bookingStatus, CustomerResponse custResInfo, string UserId)
        {
            List<CardAction> cardButtons = new List<CardAction>();
            List<CardImage> cardImages = new List<CardImage>();
            plAttachmentList.Clear();

            try
            {
                plAttachmentList.Clear();
                Dictionary<string, string> cardContentList = new Dictionary<string, string>();

                for (int i = 1; i < 6; i++)
                {
                    if (bookingStatus.flightList.Count > i && bookingStatus.destinationlist.Count > i && bookingStatus.depaturedateslist.Count > i)
                    {
                        if (!cardContentList.ContainsKey(bookingStatus.flightList[i]))
                        {
                            string d = bookingStatus.depaturedateslist[i].Substring(0, 10);
                            DateTime dt = Convert.ToDateTime(d);
                            string newdate = dt.ToString("dddd, dd MMM yyyy");
                            string time = bookingStatus.depaturedateslist[i].Substring(11, 5);

                            cards.buttonCount = 1;
                            cards.buttonValue[0] = bookingStatus.flightList[i];
                            cards.buttonType[0] = "imBack";
                            cards.buttonTitle[0] = $"DL{bookingStatus.flightList[i]}";
                            cards.HeroTitle = bookingStatus.destinationlist[i];
                            cards.HeroSubtitle = "Departs: " + newdate + " at " + time;
                            cards.HeroText = "";
                            cards.HeroImg = "";
                            custResInfo = GetHeroCard(custResInfo, cards, UserId);
                            plAttachmentList.Add(custResInfo.plAttachment);
                        }
                    }

                    else
                        break;
                }

            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: TemplateResponse ");
                throw (e);
            }
            custResInfo.plAttachment = null;
            custResInfo.plAttachmentList = plAttachmentList;
            return custResInfo;
        }
        public static JObject GetFacebookTemplate(CustomerResponse custResInfo, Customer custInfo, BookingStatus bookingStatus, FlightStatus fltStatus)
        {
            if (custResInfo.TemplateName == "Facebook-Itenary")
            {
                return JObject.FromObject(new
                {
                    attachment = new
                    {
                        type = "template",
                        payload = new
                        {
                            template_type = "airline_itinerary",
                            locale = "en_US",
                            theme_color = "#193156",
                            pnr_number = custInfo.PNR,
                            passenger_info = new[]
                    {
                     new
                     {
                      name= custInfo.FirstName,
                      ticket_number = custInfo.PNR,
                      passenger_id= "p001"
                      }
                    },
                            flight_info = new[]
                   {
                   new {
                        connection_id = "c001",
                        segment_id= "s001",
                        flight_number = custResInfo.selectedFlightNo,
                        aircraft_type = fltStatus.aircraft_type,
                        departure_airport = new
                        {
                         airport_code =bookingStatus.departureStation,
                         city = bookingStatus.departureCity,
                         terminal = fltStatus.departureTerminal,
                         gate = fltStatus.departureGate
                        },
                       arrival_airport = new
                       {
                       airport_code = custResInfo.arrivalStation,
                       city = custResInfo.arrivalCity,
                       terminal = fltStatus.arrivalTerminal,
                       gate = fltStatus.arrivalGate
                       },
                      flight_schedule = new
                      {
                      departure_time = custResInfo.departureTime,
                      arrival_time = custResInfo.arrivalTime
                      },
                      travel_class = "business"
                     }
                   },
                            passenger_segment_info = new[]
               {
               new {
                   segment_id = "s001",
                   passenger_id = "p001",
                   seat = "12A",
                   seat_type = "Business"
                   }
               },
                            price_info = new[]
          {
          new {
              title = "Fuel surcharge",
              amount = "0",
              currency = "USD"
              }
          },
                            base_price = "0",
                            tax = "0",
                            total_price = "0",
                            currency = "USD"
                        }
                    }
                });
            }
            else if (custResInfo.TemplateName == "Facebook-seatMap")
            {
                return JObject.FromObject(new
                {
                    attachment = new
                    {

                        type = "template",
                        payload = new
                        {
                            template_type = "button",
                            text = "Click the button below to view the seat map",
                            buttons = new[]
                                {
                                 new
                                 {
                                   type = "web_url",
                                   url = ""+seatmapUrl+"/?pnr="+custInfo.PNR+"&fname="+custInfo.FirstName+"&lname="+custInfo.LastName+"",
                                   title = "View Seatmap",
                                   webview_height_ratio = "full",
                                   messenger_extensions= true
                                  }
                                }
                        }
                    }
                });
            }
            else if (custResInfo.TemplateName == "Facebook-Status")
            {
                if (bookingStatus.flightStatus == "cancelled")
                {
                    status = "cancellation";
                }
                else if (bookingStatus.flightStatus == "delayed")
                {
                    status = "delay";
                }
                flightNo = "DL " + bookingStatus.flightNumber;
                return JObject.FromObject(new
                {
                    attachment = new
                    {
                        type = "template",
                        payload = new
                        {
                            template_type = "airline_update",
                            intro_message = "",
                            update_type = status,
                            locale = "en_US",
                            pnr_number = custInfo.PNR,
                            update_flight_info = new
                            {
                                flight_number = flightNo,
                                departure_airport = new
                                {
                                    airport_code = bookingStatus.departureStation,
                                    city = bookingStatus.departureCity,
                                    terminal = fltStatus.departureTerminal,
                                    gate = fltStatus.departureGate
                                },
                                arrival_airport = new
                                {
                                    airport_code = bookingStatus.arrivalStation,
                                    city = bookingStatus.arrivalCity,
                                    terminal = fltStatus.arrivalTerminal,
                                    gate = fltStatus.arrivalGate
                                },
                                flight_schedule = new
                                {
                                    departure_time = bookingStatus.departureTime,
                                    arrival_time = bookingStatus.arrivalTime
                                }
                            }
                        }
                    }
                });
            }
            else
            {
                return null;
            }
        }
    }
}