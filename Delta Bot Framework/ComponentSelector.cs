using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleBotTemplate
{
    public static class ComponentSelector
    {
        public static BaseComponents SelectChannelComponents(string channelID,string componentName)
        {
            BaseComponents obj = null;

            string componentType = ConnectionManager.getComponentType(componentName);
            switch (componentType.ToUpper())
            {
                case "TEXT":
                    obj = new TextComponents();  // new FBComponents();
                    break;
                case "CARD":
                    obj = getCard(channelID);// new SkypeComponents();
                    break;
                case "TEMPLATE":
                    obj = getTemplate(channelID);
                    break;
                default:
                    obj = new TextComponents();
                    break;
            }
            return obj;
        }

        //private static BaseComponents getCard(string channelID)
        //{
        //    BaseComponents obj = null;
        //    if (channelID.ToUpper() == "FACEBOOK")
        //    {
        //        obj = new FBComponents();
        //    }
        //    else if (channelID.ToUpper() == "SKYPE" || channelID.ToUpper() == "WEBCHAT")
        //    {
        //        obj = new SkypeComponents();
        //    }
        //    else
        //    {
        //        obj = new TextComponents();
        //    }
        //    return obj;
        //}

        //private static BaseComponents getTemplate(string channelID)
        //{
        //    BaseComponents obj = null;
        //    if (channelID.ToUpper() == "FACEBOOK")
        //    {
        //        obj = new FBComponents();
        //    }
        //    else if( channelID.ToUpper() == "WEBCHAT")
        //    {
        //        obj = new SkypeComponents();
        //    }
        //    else
        //    {
        //        obj = new TextComponents();
        //    }
        //    return obj;
        //}


        private static BaseComponents getCard(string channelID)
        {
            BaseComponents obj = null;
            if (channelID.ToUpper() == "FACEBOOK")
            {
                obj = new FBComponents();
            }
            else if (channelID.ToUpper() == "SKYPE")
            {
                obj = new SkypeComponents();
            }
            else
            {
                obj = new TextComponents();
            }
            return obj;
        }

        private static BaseComponents getTemplate(string channelID)
        {
            BaseComponents obj = null;
            if (channelID.ToUpper() == "FACEBOOK")
            {
                obj = new FBComponents();
            }
            
            else
            {
                obj = new TextComponents();
            }
            return obj;
        }
    }
}