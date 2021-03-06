// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utility.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//   This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: Ripper
//   Function  : Extracts Images posted on forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ripper.Core.Components
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using System.Xml.Serialization;

    using Ripper.Core.Objects;

    /// <summary>
    /// This page is probably the biggest mess I've ever managed to conceive.
    /// It's so nasty that I dare not even comment much.
    /// But as the file name says, it's just a bunch of non-dependant classes
    /// and functions for doing nifty little things.
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Gets a value indicating whether this instance is running on mono.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is running on mono; otherwise, <c>false</c>.
        /// </value>
        public static bool IsRunningOnMono
        {
            get
            {
                return Type.GetType("Mono.Runtime") != null;
            }
        }
        
        /// <summary>
        /// Check the FilePath for Length because if its more then 260 characters long it will crash
        /// </summary>
        /// <param name="filePath">
        /// Folder Path to check
        /// </param>
        /// <returns>
        /// The check path length.
        /// </returns>
        public static string CheckPathLength(string filePath)
        {
            if (filePath.Length <= 260)
            {
                return filePath;
            }
            
            var shortFilePath = filePath.Substring(filePath.LastIndexOf("\\", StringComparison.Ordinal) + 1);

            filePath = Path.Combine(CacheController.Instance().UserSettings.DownloadFolder, shortFilePath);

            return filePath;
        }
        
        /// <summary>
        /// Although these are not hex, but rather html codes for special characters
        /// </summary>
        /// <param name="inputString">
        /// String to check
        /// </param>
        /// <returns>
        /// The replace hex with ASCII.
        /// </returns>
        public static string ReplaceHexWithAscii(string inputString)
        {
            if (inputString == null)
            {
                return string.Empty;
            }

            inputString = inputString.Replace("&amp;amp;", "&");
            inputString = inputString.Replace("&amp;", "&");
            inputString = inputString.Replace("&quot;", "''");
            inputString = inputString.Replace("&lt;", string.Empty);
            inputString = inputString.Replace("&gt;", string.Empty);
            inputString = inputString.Replace("�", "e");
            inputString = inputString.Replace("\t", string.Empty);
            inputString = inputString.Replace("@", "at");

            return inputString;
        }

       /// <summary>
        /// It's essential to give files legal names. Otherwise the Win32API 
        /// sends back a bucket full of cow dung.
        /// </summary>
        /// <param name="inputString">
        /// String to check
        /// </param>
        /// <returns>
        /// The remove illegal characters.
        /// </returns>
        public static string RemoveIllegalCharecters(string inputString)
        {
            inputString = inputString.Replace("&amp;amp;", "&");
            inputString = inputString.Replace("\\", string.Empty);
            inputString = inputString.Replace("/", "-");
            inputString = inputString.Replace("*", "+");
            inputString = inputString.Replace("?", string.Empty);
            inputString = inputString.Replace("!", string.Empty);
            inputString = inputString.Replace("\"", "'");
            inputString = inputString.Replace("<", "(");
            inputString = inputString.Replace(">", ")");
            inputString = inputString.Replace("|", "!");
            inputString = inputString.Replace(":", ";");
            inputString = inputString.Replace("&amp;", "&");
            inputString = inputString.Replace("&quot;", "''");
            inputString = inputString.Replace("&apos;", "'");
            inputString = inputString.Replace("&lt;", string.Empty);
            inputString = inputString.Replace("&gt;", string.Empty);
            inputString = inputString.Replace("�", "e");
            inputString = inputString.Replace("\t", string.Empty);
            inputString = inputString.Replace("@", "at");
            inputString = inputString.Replace("\r", string.Empty);
            inputString = inputString.Replace("\n", string.Empty);

            return inputString;
        }

        /// <summary>
        /// Checks to see if a file already exists at destination
        /// that's of the same name. If so, it incrementally adds numerical
        /// values prior to the image extension until the new file path doesn't
        /// already have a file there.
        /// </summary>
        /// <param name="path">Image path</param>
        /// <param name="customName">Indicator if this is a custom name</param>
        /// <returns>
        /// The get suitable name.
        /// </returns>
        public static string GetSuitableName(string path, bool customName = false)
        {
            var newAlteredPath = path;
            var renameCount = 1;

            var begining = newAlteredPath.Substring(0, newAlteredPath.LastIndexOf(".", StringComparison.Ordinal));
            var end = newAlteredPath.Substring(newAlteredPath.LastIndexOf(".", StringComparison.Ordinal));

            if (customName)
            {
                newAlteredPath = string.Format("{0}_{1:000}{2}", begining, renameCount, end);
            }

            while (File.Exists(newAlteredPath))
            {
                if (customName)
                {
                    begining = begining.Substring(0, newAlteredPath.LastIndexOf("_", StringComparison.Ordinal));
                }

                newAlteredPath = string.Format("{0}_{1:000}{2}", begining, renameCount, end);
                renameCount++;
            }

            return newAlteredPath;
        }

        /// <summary>
        /// Check if Input is a Numeric Value (Numbers)
        /// </summary>
        /// <param name="valueToCheck">
        /// The value To Check.
        /// </param>
        /// <returns>
        /// Return if Numeric or not
        /// </returns>
        public static bool IsNumeric(object valueToCheck)
        {
            double dummy;
            var inputValue = Convert.ToString(valueToCheck);

            return double.TryParse(inputValue, NumberStyles.Any, null, out dummy);
        }

        /// <summary>
        /// This function allows or disallows the inclusion of an image for fetching.
        /// returning true DISALLOWS the image from inclusion...
        /// </summary>
        /// <param name="imagePath">
        /// The image Path.
        /// </param>
        /// <returns>
        /// The is image none sense.
        /// </returns>
        public static bool IsImageNoneSense(string imagePath)
        {
            return imagePath.ToLower().Contains("rip.bamva")
                   || (imagePath.Contains(@"Smilies") || imagePath.Contains(@"emoticons")
                       || imagePath.Contains("forum.php") || imagePath.Contains("images/misc/")
                       || imagePath.Contains("images/pagination/") || imagePath.Contains("showthread.php"));
        }

        /// <summary>
        /// Encrypts a password using MD5.
        /// not my code in this function, but falls under public domain.
        /// Author unknown. But Thanks to the author none the less.
        /// </summary>
        /// <param name="originalPass">
        /// The original Pass.
        /// </param>
        /// <returns>
        /// The encoded password.
        /// </returns>
        public static string EncodePassword(string originalPass)
        {
            var md5 = new MD5CryptoServiceProvider();

            var originalBytes = Encoding.Default.GetBytes(originalPass);
            var encodedBytes = md5.ComputeHash(originalBytes);

            // Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }

        /// <summary>
        /// Gets the forum page as string.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// The Page Content
        /// </returns>
        public static string GetForumPageAsString(string url)
        {
            string pageContent;

            try
            {
                var webClient = new WebClient();

                if (!CacheController.Instance().UserSettings.GuestMode)
                {
                    webClient.Headers.Add(string.Format("Cookie: {0}", CookieManager.GetInstance().GetCookieString()));
                }

                pageContent = webClient.DownloadString(url);

                webClient.Dispose();
            }
            catch (Exception)
            {
                pageContent = string.Empty;
            }

            return pageContent;
        }

        /// <summary>
        /// Save all Jobs, and the current one which causes the crash to a CrashLog_...txt
        /// </summary>
        /// <param name="exceptionMessage">Exception Message</param>
        /// <param name="stackTrace">Exception Stack Trace</param>
        /// <param name="currentJobInfo">Current Download Job</param>
        public static void SaveOnCrash(string exceptionMessage, string stackTrace, JobInfo currentJobInfo)
        {
            const string ErrMessage =
                "An application error occurred. Please contact Admin (https://ripper.codeplex.com/workitem/list/basic) "
                + "with the following information:";

            var currentDateTime = ReplaceHexWithAscii(DateTime.Now.ToString("G", CultureInfo.InvariantCulture));

            // Save Current Job and the Error to txt file
            var logFileName = string.Format("Crash_{0}.txt", currentDateTime.Replace("/", "-").Replace(" ", "_").Replace(":", "."));

            // Save Current Job and the Error to txt file
            var file = new FileStream(Path.Combine(Application.StartupPath, logFileName), FileMode.CreateNew);
            var sw = new StreamWriter(file);
            sw.WriteLine(ErrMessage);
            sw.Write(sw.NewLine);
            sw.Write(exceptionMessage);
            sw.Write(sw.NewLine);
            sw.Write(sw.NewLine);
            sw.WriteLine("Stack Trace:");
            sw.Write(sw.NewLine);
            sw.Write(stackTrace);
            sw.Write(sw.NewLine);
            sw.Write(sw.NewLine);

            if (currentJobInfo != null)
            {
                sw.WriteLine("Current Job DUMP:");
                sw.Write(sw.NewLine);

                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sw.WriteLine(
                    "<ArrayOfJobInfo xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
                sw.WriteLine("  <JobInfo>");
                sw.WriteLine("    <StorePath>{0}</StorePath>", currentJobInfo.StorePath);
                sw.WriteLine("    <Title>{0}</Title>", currentJobInfo.TopicTitle);
                sw.WriteLine("    <PostTitle>{0}</PostTitle>", currentJobInfo.PostTitle);
                sw.WriteLine("    <ForumTitle>{0}</ForumTitle>", currentJobInfo.ForumTitle);
                sw.WriteLine("    <URL>{0}</URL>", currentJobInfo.XMLUrl);
                sw.WriteLine("    <XMLPayLoad>{0}</XMLPayLoad>", currentJobInfo.XMLPayLoad);
                sw.WriteLine("    <ImageCount>{0}</ImageCount>", currentJobInfo.ImageCount);
                sw.WriteLine("  </JobInfo>");
                sw.WriteLine("</ArrayOfJobInfo>");
            }

            sw.Close();
            file.Close();
        }

        /// <summary>
        /// Gets the Security Token for the Thank You Button
        /// </summary>
        /// <param name="pageUrl">URL of the Post</param>
        /// <returns>The Security Token</returns>
        public static string GetSecurityToken(Uri pageUrl)
        {
            var webClient = new WebClient();

            webClient.Headers.Add(string.Format("Referer: {0}", pageUrl));

            if (!CacheController.Instance().UserSettings.GuestMode)
            {
                webClient.Headers.Add(string.Format("Cookie: {0}", CookieManager.GetInstance().GetCookieString()));
            }

            return GetSecurityToken(webClient.DownloadString(pageUrl));
        }

        /// <summary>
        /// Gets the Security Token for the Thank You Button
        /// </summary>
        /// <param name="pageContent">Content of the page.</param>
        /// <returns>
        /// The Security Token
        /// </returns>
        public static string GetSecurityToken(string pageContent)
        {
            var securityToken = pageContent;

            var match = Regex.Match(
                securityToken,
                @"var SECURITYTOKEN = \""(?<inner>[^\""]*)\"";",
                RegexOptions.Compiled);

            if (match.Success)
            {
                securityToken = match.Groups["inner"].Value;
            }

            return securityToken;
        }

        /// <summary>
        /// Exports the current jobs queue.
        /// </summary>
        /// <param name="savePath">The save path.</param>
        /// <param name="jobsList">The jobs list.</param>
        public static void ExportCurrentJobsQueue(string savePath, List<JobInfo> jobsList)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<JobInfo>));
            TextWriter tr = new StreamWriter(savePath);
            serializer.Serialize(tr, jobsList);
            tr.Close();
        }
    }
}