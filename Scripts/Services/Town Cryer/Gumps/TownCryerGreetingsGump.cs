using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Engines.Quests;
using Server.Gumps;

namespace Server.Services.TownCryer
{
    public class TownCryerGreetingsGump : BaseTownCryerGump
    {
        public int Page { get; private set; }
        public int Pages { get { return TownCryerSystem.GreetingsEntries.Count; } }

        public TownCryerGreetingEntry Entry { get; private set; }

        public TownCryerGreetingsGump(PlayerMobile pm, TownCrier cryer, int page = 0)
            : base(pm, cryer)
        {
            Page = page;
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            var list = TownCryerSystem.GreetingsEntries;
            Entry = TownCryerSystem.GreetingsEntries[0];

            if (Page >= 0 && Page < list.Count)
            {
                Entry = list[Page];
            }

            int y = 150;

            if (Entry.Title != null)
            {
                if (Entry.Title.Number > 0)
                {
                    AddHtmlLocalized(78, y, 700, 400, Entry.Title.Number, false, false);
                }
                else
                {
                    AddHtml(78, y, 700, 400, Entry.Title.ToString(), false, false);
                }

                y += 40;
            }

            if (Entry.Body.Number > 0)
            {
                AddHtmlLocalized(78, y, 700, 400, Entry.Body.Number, false, false);
            }
            else
            {
                AddHtml(78, y, 700, 400, Entry.Body.ToString(), false, false);
            }

            if (Entry.Expires != DateTime.MinValue)
            {
                AddHtmlLocalized(50, 550, 200, 20, 1060658, String.Format("{0}\t{1}", "Created", Entry.Created.ToShortDateString()), 0, false, false);
                AddHtmlLocalized(50, 570, 200, 20, 1060659, String.Format("{0}\t{1}", "Expires", Entry.Expires.ToShortDateString()), 0, false, false);
            }

            AddButton(350, 570, 0x605, 0x606, 1, GumpButtonType.Reply, 0);
            AddButton(380, 570, 0x609, 0x60A, 2, GumpButtonType.Reply, 0);
            AddButton(430, 570, 0x607, 0x608, 3, GumpButtonType.Reply, 0);
            AddButton(455, 570, 0x603, 0x604, 4, GumpButtonType.Reply, 0);

            AddHtml(395, 570, 35, 20, Center(String.Format("{0}/{1}", (Page + 1).ToString(), Pages.ToString())), false, false);

            AddButton(525, 625, 0x5FF, 0x600, 5, GumpButtonType.Reply, 0);
            AddHtmlLocalized(550, 625, 300, 20, 1158386, false, false); // Close and do not show this version again

            if (Entry.Link != null)
            {
                if (!string.IsNullOrEmpty(Entry.LinkText))
                {
                    AddHtml(50, 490, 745, 40, String.Format("<a href=\"{0}\">{1}</a>", Entry.Link, Entry.LinkText), false, false);
                }
                else
                {
                    AddHtml(50, 490, 745, 40, String.Format("<a href=\"{0}\">{1}</a>", Entry.Link, Entry.Link), false, false);
                }
            }

            /*if (TownCryerSystem.HasCustomEntries())
            {
                AddButton(40, 615, 0x603, 0x604, 6, GumpButtonType.Reply, 0);
                AddHtmlLocalized(68, 615, 300, 20, 1060660, String.Format("{0}\t{1}", "Sort By", Sort.ToString()), 0, false, false);
            }*/

            if (User.AccessLevel >= AccessLevel.Administrator)
            {
                AddButton(40, 640, 0x603, 0x604, 7, GumpButtonType.Reply, 0);
                AddHtml(68, 640, 300, 20, "Entry Props", false, false);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            int button = info.ButtonID;

            switch (button)
            {
                case 0: break;
                case 1: // <<
                    Page = 0;
                    Refresh();
                    break;
                case 2: // <
                    Page = Math.Max(0, Page - 1);
                    Refresh();
                    break;
                case 3: // >
                    Page = Math.Min(Pages - 1, Page + 1);
                    Refresh();
                    break;
                case 4: // >>
                    Page = Pages - 1;
                    Refresh();
                    break;
                case 5: // No Show
                    TownCryerSystem.AddExempt(User);
                    break;
                case 7:
                    Refresh();

                    if (Entry != null)
                    {
                        User.SendGump(new PropertiesGump(User, Entry));
                    }
                    break;
            }
        }
    }
}