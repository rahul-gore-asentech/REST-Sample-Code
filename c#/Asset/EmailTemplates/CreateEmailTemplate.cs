/*
   CreateEmailTemplate.cs

   Marketo REST API Sample Code
   Copyright (C) 2016 Marketo, Inc.

   This software may be modified and distributed under the terms
   of the MIT license.  See the LICENSE file for details.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections;
using Newtonsoft.Json;
using System.Web;

namespace Samples
{

    class CreateEmailTemplate
    {
        private String host = "CHANGE ME"; //host of your marketo instance, https://AAA-BBB-CCC.mktorest.com
        private String clientId = "CHANGE ME"; //clientId from admin > Launchpoint
        private String clientSecret = "CHANGE ME"; //clientSecret from admin > Launchpoint
        public String name;//name of new template, required
        public Dictionary<string, dynamic> folder;//dict with two members, id and type, must be Folder, required
        public String content;//html content of the template, required
        public String description;// optional description of new template

        /*
        public static void Main(string[] args)
        {
            var template = new CreateEmailTemplate();
            template.name = "HTML Example Template";
            template.folder = new Dictionary<string, dynamic>();
            template.folder.Add("id", 15);
            template.folder.Add("type", "Folder");
            template.content = File.ReadAllText("C:\\EmailContent\\template.html");
            String result = template.postData();
            Console.Write(result);
        }
        */

        public String postData()
        {
            //Assemble the URL
            String url = host + "/rest/asset/v1/emailTemplates.json?access_token=" + getToken();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            String boundary = "--mktoBoundary" + DateTime.Now.Ticks.ToString("x");
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Accept = "application/json";
            StreamWriter wr = new StreamWriter(request.GetRequestStream());
            //StreamWriter wr = new StreamWriter(Console.Out);
            //TextWriter wr = Console.Out;
            AddMultipartFile(wr, boundary, "text/html", "content", content, "template.html");
            AddMultipartParam(wr, boundary, "name", name);
            AddMultipartParam(wr, boundary, "folder", JsonConvert.SerializeObject(folder));
            if (description != null)
            {
                AddMultipartParam(wr, boundary, "description", description);
            }
            wr.Write("--" + boundary + "--");
            wr.Flush();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(resStream);
            return reader.ReadToEnd();
        }
        private void AddMultipartFile(TextWriter wr, String boundary, String contentType, String paramName, String content, String filename)
        {
            wr.Write("--" + boundary + "\r\n");
            wr.Write("Content-Disposition: form-data; name=\"" + paramName + "\"; filename=\"" + filename + "\"\r\n");
            wr.Write("Content-type: text/html; charset=\"utf-8\"\r\n");
            wr.Write("\r\n");
            wr.Write(content + "\r\n");
        }
        private void AddMultipartParam(TextWriter wr, String boundary, String paramName, String content)
        {
            wr.Write("--" + boundary + "\r\n");
            wr.Write("Content-Disposition: form-data; name=\"" + paramName + "\"\r\n");
            wr.Write("\r\n");
            wr.Write(content + "\r\n");
        }
        
        private String getToken()
        {
            String url = host + "/identity/oauth/token?grant_type=client_credentials&client_id=" + clientId + "&client_secret=" + clientSecret;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(resStream);
            String json = reader.ReadToEnd();
            //Dictionary<String, Object> dict = JavaScriptSerializer.DeserializeObject(reader.ReadToEnd);
            Dictionary<String, String> dict = JsonConvert.DeserializeObject<Dictionary<String, String>>(json);
            return dict["access_token"];
        }
    }
}
