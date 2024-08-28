﻿using Asset.API.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Asset.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        IWebHostEnvironment _env;

        public ErrorController(IWebHostEnvironment env)
        {

            _env = env;

        }

        [HttpGet]
        [Route("GetMacAddress")]
        public IActionResult GetMacAddress()
        {
            string firstMacAddress = NetworkInterface.GetAllNetworkInterfaces()
                                         .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                                         .Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

            string textFile = Path.Combine(_env.ContentRootPath, "UploadedAttachments") + "\\" + "MAC.abc";
            if (System.IO.File.Exists(textFile))
            {
                FileInfo file = new FileInfo(textFile);
                file.Attributes &= ~FileAttributes.Hidden;

                using (StreamReader streamfile = new StreamReader(textFile))
                {
                    int counter = 0;
                    string ln;
                    while ((ln = streamfile.ReadLine()) != null)
                    {
                        if (ln != firstMacAddress)
                        {
                            System.IO.File.SetAttributes(textFile, FileAttributes.Hidden);
                            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "error", Message = "", MessageAr = "" });
                        }
                        counter++;
                    }
                    streamfile.Close();
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "error", Message = "", MessageAr = "" });
            }
            return Ok();
        }




        [HttpGet]
        [Route("GetMBSerial")]
        public IActionResult GetMBSerial()
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "wmic",
                    Arguments = "baseboard get SerialNumber",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            //   /DR9L6L3/CNFCW001BA008O/
            process.Start();
            string serialNumber, serialOutPut = "";
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (output.Contains("/"))
            {
            //    string input = "/DR9L6L3/CNFCW001BA008O/";
                serialNumber = output.Trim();
                string[] parts = serialNumber.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                serialOutPut = parts[^1]; // Using the index -1 to access the last element
            }
            else
            {
                serialNumber = output.Trim();
                char delimiter = '\n';
                string[] substringsSerial = serialNumber.Split(delimiter);
                serialOutPut = substringsSerial[1];
            }

            string textFile = Path.Combine(_env.ContentRootPath, "UploadedAttachments") + "\\" + "MAC.abc";
            if (System.IO.File.Exists(textFile))
            {
                FileInfo file = new FileInfo(textFile);
                file.Attributes &= ~FileAttributes.Hidden;

                using (StreamReader streamfile = new StreamReader(textFile))
                {
                    int counter = 0;
                    string ln;
                    while ((ln = streamfile.ReadLine()) != null)
                    {
                        if (ln != serialOutPut)
                        {
                            System.IO.File.SetAttributes(textFile, FileAttributes.Hidden);
                            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "error", Message = "", MessageAr = "" });
                        }
                        counter++;
                    }
                    streamfile.Close();
                }
            }
            else
            {
                System.IO.File.SetAttributes(textFile, FileAttributes.Hidden);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "error", Message = "", MessageAr = "" });
            }

            return Ok();
        }
    }
}
