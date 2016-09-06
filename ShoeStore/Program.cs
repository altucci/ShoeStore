/*-------------------------------------------------------

    Author: Albert Tucci


    The purpose of this applicaiton is to verify the existence of
    certain key elements within http://shoestore-manheim.rhcloud.com/
    

    Story 1: (Monthly display of new releases)
        In order to view upcoming shoes being released every month As a user of the
        Shoe store I want to be able to visit a link for each month and see the shoes being released

        [Acceptance Criteria]
            1. Month should display a small Blurb of each shoe
            2. Month should display an image each shoe being released
            3. Each shoe should have a suggested price pricing 


    Story 2: (Submit email for reminder)
        In order to be reminded of upcoming shoe releases As a user of the Shoe Store I want to be
        able to submit my email address

        [Acceptance Criteria]
            1. There should be an area to submit email address on successful submission of a valid
               email address user should receive a message Thanks! We will notify you of our new shoes
               at this email: users email address

-------------------------------------------------------*/



using System;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Text;

namespace ShoeStore
{
    class Program
    {
        internal string strBaseURL;
        HtmlNodeCollection MonthlyURLs;
        HtmlNodeCollection Shoes;



        /*-------------------------------------------------------
            Default Constructor
        -------------------------------------------------------*/
        Program()
        {
            strBaseURL = "";
        }



        /*-------------------------------------------------------
            Constructor. Sets the base path URL.
        -------------------------------------------------------*/
        Program(string strURL)
        {
            strBaseURL = strURL;
        }



        /*-------------------------------------------------------
        Private

            Receives a string as a parameter and removes some irregular
            characters from said string.
            
            Returns the modified string.
        -------------------------------------------------------*/
        internal string TrimString(string str)
        {
            str = str.Trim();
            str = str.Replace("\n", "");
            str = str.Replace("\r", "");
            str = str.Replace("\t", "");

            return str;
        }

        

        /*-------------------------------------------------------
        Private

            Sends a GET Http request to the URL provided in the
            parameter strURL.

            Receives all of the incoming Html response data.

            Returns a string representing the response data.
        -------------------------------------------------------*/
        internal string ReadHtmlData(string strURL)
        {
            string html = "";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
                request.Method = "GET";
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                html = reader.ReadToEnd();
                reader.Close();
                response.Close();
            }
            catch (Exception e)
            {

            }
            finally
            {

            }

            return html;
        }



        /*-------------------------------------------------------
        Private

            Sends a HEAD Http request to the image URL provided in the
            parameter strImageURL.

            Returns a bool value of true if the image exist at the path
            given, and a value of false if the image does not exist at the
            path given.

            If strImageURL is empty, a value of false is returned.
        -------------------------------------------------------*/
        internal bool DoesImageExist(string strImageURL)
        {
            bool retValid = false;

            HttpWebResponse response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strImageURL);
                request.Method = "HEAD";
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                response = null;
            }
            finally
            {
                if (response != null)
                {
                    retValid = true;
                    response.Close();
                }
            }

            return retValid;
        }



        /*-------------------------------------------------------
        Private

            Validates each element in a shoe node. Evaluates an entire
            list of shoe nodes for a given month.

            Evaluations are displayed on the console window.

            The month is provided as a parameter.
        -------------------------------------------------------*/
        internal void ValidateShoes(HtmlNode Month)
        {
            if (Shoes != null)
            {
                Console.WriteLine(TrimString(Month.InnerText) + " - " + Shoes.Count.ToString() + " Items\n");

                foreach (HtmlNode n in Shoes)
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(n.WriteTo());

                    HtmlNode Brand = doc.DocumentNode.SelectSingleNode("//td[@class='shoe_result_value shoe_brand'] //a");
                    string strBrand = TrimString(Brand.InnerText);
                    HtmlNode Name = doc.DocumentNode.SelectSingleNode("//td[@class='shoe_result_value shoe_name']");
                    string strName = TrimString(Name.InnerText);
                    Console.WriteLine("\t" + strBrand + " - " + strName);

                    HtmlNode Description = doc.DocumentNode.SelectSingleNode("//td[@class='shoe_result_value shoe_description']");
                    string strDescription = TrimString(Description.InnerText);
                    Console.Write("\tDoes Blurb Exist:  ");
                    if (strDescription != "")
                    {
                        Console.WriteLine("YES");
                    }
                    else
                    {
                        Console.WriteLine("NO");
                    }

                    HtmlNode ImageURL = doc.DocumentNode.SelectSingleNode("//td[@class='shoe_image'] //img");
                    string strImageURL = TrimString(ImageURL.GetAttributeValue("src", ""));
                    Console.Write("\tDoes Image Exist:  ");
                    if (DoesImageExist(strImageURL))
                    {
                        Console.WriteLine("YES");
                    }
                    else
                    {
                        Console.WriteLine("NO");
                    }

                    HtmlNode Price = doc.DocumentNode.SelectSingleNode("//td[@class='shoe_result_value shoe_price']");
                    string strPrice = TrimString(Price.InnerText);
                    Console.Write("\tDoes Price Exist:  ");
                    if (strPrice != "")
                    {
                        Console.WriteLine("YES");
                    }
                    else
                    {
                        Console.WriteLine("NO");
                    }

                    Console.Write("\n");
                }

                Console.Write("\n\n");
            }
            else
            {
                Console.WriteLine(TrimString(Month.InnerText) + " - 0 Items\n\n\n");
            }
        }



        /*-------------------------------------------------------
            Requests all Html data from strBaseURL.

            Parses requested data to find and store all URLs
            coresponding to the hyperlinks representing the 12 months
            of the year.
        -------------------------------------------------------*/
        public void ParseMonthlyURLs()
        {
            string html = ReadHtmlData(strBaseURL);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            MonthlyURLs = doc.DocumentNode.SelectNodes("//div[@id='header_nav'] //li //a");
        }



        /*-------------------------------------------------------
            Attempts to validate every shoe element in ever shoe node
            in every month.
            
            Loops through all monthly URLs.

            Each loop iteration:
                1. The given month's html data is requested.
                2. All shoe nodes are parsed out for the given month.
                3. All elements in each shoe node are checked for
                   validity.
                4. The month increments for the next loop iteration.
        -------------------------------------------------------*/
        public void ValidateShoeLists()
        {
            Console.Write("\n\n\n");

            foreach (HtmlNode nodes in MonthlyURLs)
            {
                string html = ReadHtmlData(strBaseURL + nodes.GetAttributeValue("href", ""));
                
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                Shoes = doc.DocumentNode.SelectNodes("//div[@class='shoe_result']");

                HtmlDocument doc2 = new HtmlDocument();
                doc2.LoadHtml(html);
                HtmlNode Month = doc2.DocumentNode.SelectSingleNode("//div[@class='title'] //h2");

                ValidateShoes(Month);
            }
        }



        /*-------------------------------------------------------
            Attempts to validate the Email Reminder form.
            
            Sends a POST Http request to the URL provided in the
            parameter strURL.

            Receives all of the incoming Html response data.

            Finds the StatusCode within the response data, and makes
            a determination on the success of the POST request based
            on this StatusCode.
        -------------------------------------------------------*/
        public void ValidateEmailForm()
        {
            WebRequest req = WebRequest.Create(strBaseURL + "/remind");
            req.Method = "POST";
            string postData = "email=test153928647@gmail.com";      //A dummy email address
            byte[] bArr = Encoding.UTF8.GetBytes(postData);
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bArr.Length;
            Stream dataStream = req.GetRequestStream();
            dataStream.Write(bArr, 0, bArr.Length);
            dataStream.Close();

            WebResponse response = null;
            try
            {
                response = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                response = (HttpWebResponse)e.Response;
            }
            
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseServer = reader.ReadToEnd();
            //Console.WriteLine(responseServer);

            reader.Close();
            dataStream.Close();
            response.Close();

            int rCode = (int)((HttpWebResponse)response).StatusCode;

            if (rCode == 200)
                Console.WriteLine("The Email Reminder was successful.  StatusCode: " + rCode + " (" + ((HttpWebResponse)response).StatusDescription + ")\n\n\n");
            else
                Console.WriteLine("The Email Reminder was unsuccessful.  StatusCode: " + rCode + " (" + ((HttpWebResponse)response).StatusDescription + ")\n\n\n");
        }



        static void Main(string[] args)
        {
            Console.WriteLine("Loading data from site...");
            
            //Create instance of class Program
            Program p = new Program("http://shoestore-manheim.rhcloud.com");



            //Parse out all hyperlinks coresponding to a month in the year
            p.ParseMonthlyURLs();
            //Validate all shoe elements in all shoe nodes for every month
            p.ValidateShoeLists();
            //Validate Email Reminder form
            p.ValidateEmailForm();



            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
