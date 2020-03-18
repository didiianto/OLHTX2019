using iTextSharp.text.pdf;
using OLHTX2019.Models.DB;
using OLHTX2019.Models.EntityManager;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Zen.Barcode;

namespace OLHTX2019.Helper
{
    public enum RegStatus
    {
        DFT,
        CHK,
        PEN,
        OK
    }
    public enum RegStepAction
    {
        DFT,
        INVT,
        READ,
        CLICK,
        CPLT
    }

    public class AdminAct
    {
        public const string Edit = "Edit";
        public const string AddNew = "New";
        public const string Login = "Login";
        public const string Reinvite = "Re-Invite";
        public const string GenerateReport = "Generate Report";
    }

    public class Module
    {
        public const string Admin = "Admin";
        public const string FrontEnd = "Front End";
    }

    public static class VarRef
    {
        public static List<string> Country = new List<string>
        {
            "Afghanistan",
            "Albania",
            "Algeria",
            "American Samoa",
            "Andorra",
            "Angola",
            "Anguilla",
            "Antarctica",
            "Antigua and Barbuda",
            "Argentina",
            "Armenia",
            "Aruba",
            "Australia",
            "Austria",
            "Azerbaijan",
            "Bahamas",
            "Bahrain",
            "Bangladesh",
            "Barbados",
            "Belarus",
            "Belgium",
            "Belize",
            "Benin",
            "Bermuda",
            "Bhutan",
            "Bolivia, Plurinational State Of",
            "Bonaire, Sint Eustatius and Saba",
            "Bosnia and Herzegovina",
            "Botswana",
            "Bouvet Island",
            "Brazil",
            "British Indian Ocean Territory",
            "Brunei Darussalam",
            "Bulgaria",
            "Burkina Faso",
            "Burundi",
            "Cambodia",
            "Cameroon",
            "Canada",
            "Cape Verde",
            "Cayman Islands",
            "Central African Republic",
            "Chad",
            "Chile",
            "China",
            "Christmas Island",
            "Cocos (Keeling) Islands",
            "Colombia",
            "Comoros",
            "Congo",
            "Congo, The Democratic Republic Of The",
            "Cook Islands",
            "Costa Rica",
            "Croatia",
            "Cuba",
            "Curaçao",
            "Cyprus",
            "Czech Republic",
            "Côte D'Ivoire",
            "Denmark",
            "Djibouti",
            "Dominica",
            "Dominican Republic",
            "Ecuador",
            "Egypt",
            "El Salvador",
            "Equatorial Guinea",
            "Eritrea",
            "Estonia",
            "Ethiopia",
            "Falkland Islands (Malvinas)",
            "Faroe Islands",
            "Fiji",
            "Finland",
            "France",
            "French Guiana",
            "French Polynesia",
            "French Southern Territories",
            "Gabon",
            "Gambia",
            "Georgia",
            "Germany",
            "Ghana",
            "Gibraltar",
            "Greece",
            "Greenland",
            "Grenada",
            "Guadeloupe",
            "Guam",
            "Guatemala",
            "Guernsey",
            "Guinea",
            "Guinea-Bissau",
            "Guyana",
            "Haiti",
            "Heard Island and McDonald Islands",
            "Holy See (Vatican City State)",
            "Honduras",
            "Hong Kong",
            "Hungary",
            "Iceland",
            "India",
            "Indonesia",
            "Iran, Islamic Republic Of",
            "Iraq",
            "Ireland",
            "Isle of Man",
            "Israel",
            "Italy",
            "Jamaica",
            "Japan",
            "Jersey",
            "Jordan",
            "Kazakhstan",
            "Kenya",
            "Kiribati",
            "Korea, Democratic People's Republic Of",
            "Korea, Republic of",
            "Kosovo",
            "Kuwait",
            "Kyrgyzstan",
            "Lao People's Democratic Republic",
            "Latvia",
            "Lebanon",
            "Lesotho",
            "Liberia",
            "Libya",
            "Liechtenstein",
            "Lithuania",
            "Luxembourg",
            "Macao",
            "Macedonia, the Former Yugoslav Republic Of",
            "Madagascar",
            "Malawi",
            "Malaysia",
            "Maldives",
            "Mali",
            "Malta",
            "Marshall Islands",
            "Martinique",
            "Mauritania",
            "Mauritius",
            "Mayotte",
            "Mexico",
            "Micronesia, Federated States Of",
            "Moldova, Republic of",
            "Monaco",
            "Mongolia",
            "Montenegro",
            "Montserrat",
            "Morocco",
            "Mozambique",
            "Myanmar",
            "Namibia",
            "Nauru",
            "Nepal",
            "Netherlands",
            "New Caledonia",
            "New Zealand",
            "Nicaragua",
            "Niger",
            "Nigeria",
            "Niue",
            "Norfolk Island",
            "Northern Mariana Islands",
            "Norway",
            "Oman",
            "Pakistan",
            "Palau",
            "Palestinian Territory, Occupied",
            "Panama",
            "Papua New Guinea",
            "Paraguay",
            "Peru",
            "Philippines",
            "Pitcairn",
            "Poland",
            "Portugal",
            "Puerto Rico",
            "Qatar",
            "Romania",
            "Russian Federation",
            "Rwanda",
            "Réunion",
            "Saint Barthélemy",
            "Saint Helena, Ascension and Tristan Da Cunha",
            "Saint Kitts And Nevis",
            "Saint Lucia",
            "Saint Martin (French Part)",
            "Saint Pierre And Miquelon",
            "Saint Vincent And The Grenadines",
            "Samoa",
            "San Marino",
            "Sao Tome and Principe",
            "Saudi Arabia",
            "Senegal",
            "Serbia",
            "Seychelles",
            "Sierra Leone",
            "Singapore",
            "Sint Maarten (Dutch part)",
            "Slovakia",
            "Slovenia",
            "Solomon Islands",
            "Somalia",
            "South Africa",
            "South Georgia and the South Sandwich Islands",
            "South Sudan",
            "Spain",
            "Sri Lanka",
            "Sudan",
            "Suriname",
            "Svalbard And Jan Mayen",
            "Swaziland",
            "Sweden",
            "Switzerland",
            "Syrian Arab Republic",
            "Taiwan",
            "Tajikistan",
            "Tanzania, United Republic of",
            "Thailand",
            "Timor-Leste",
            "Togo",
            "Tokelau",
            "Tonga",
            "Trinidad and Tobago",
            "Tunisia",
            "Turkey",
            "Turkmenistan",
            "Turks and Caicos Islands",
            "Tuvalu",
            "Uganda",
            "Ukraine",
            "United Arab Emirates",
            "United Kingdom",
            "United States",
            "United States Minor Outlying Islands",
            "Uruguay",
            "Uzbekistan",
            "Vanuatu",
            "Venezuela, Bolivarian Republic of",
            "Vietnam",
            "Virgin Islands, British",
            "Virgin Islands, U.S.",
            "Wallis and Futuna",
            "Western Sahara",
            "Yemen",
            "Zambia",
            "Zimbabwe",
            "Åland Islands"
        };

        public static readonly Dictionary<string, string> dtRegStatus = new Dictionary<string, string>
        {
            { "",""},
            { "DFT","Draft"},
            { "CHK","Check Out"},
            { "PEN","Pending"},
            { "OK","OK"}
        };

        public static readonly Dictionary<int, string> dtRegProgress = new Dictionary<int, string>
        {
            { 1, "POINT OF CONTACT (2 MIN)" },
            { 2, "EVENT SELECTION (10 MIN)" },
            { 3, "DELEGATE INFORMATION (8 MIN)" },
            { 4, "FLIGHT &amp; ACCOMMODATION (5 MIN)" },
            { 5, "REVIEW (5 MIN)" },
            { 6, "COMPLETE" }
        };

        public static readonly Dictionary<string, string> dtSalutation = new Dictionary<string, string>
        {
            { "1", "Dr" },
            { "2", "Mr" },
            { "3", "Ms" },
            { "4", "Prof" }
        };

        public static readonly Dictionary<string, string> dtPositionInOrg = new Dictionary<string, string>
        {
            { "1", "Senior Management" },
            { "2", "Middle Management" },
            { "3", "Junior Management" }
        };

        public static readonly Dictionary<string, string> dtIndustryType = new Dictionary<string, string>
        {
            { "1", "Academia" },
            { "2", "Clean Energy" },
            { "3", "Consultancy" },
            { "4", "Downstream" },
            { "5", "Finance" },
            { "6", "Government" },
            { "7", "Oil & Gas" },
            { "8", "Organisations & Associations" },
            { "9", "Power Generation" }
        };
        public static readonly Dictionary<string, string> dtDelegateType = new Dictionary<string, string>
        {
           { "16", "1-Partner Bundle" },
            { "15", "2-Partner Bundle" },
            { "14", "3-Partner Bundle aka \"Master Pass\"" },
            { "10", "Educational Institution" },
            { "9", "Government Agencies" },
            { "2", "HAPUA" },
            { "4", "Media Partners" },
            { "8", "MTI & Sister Agencies & EMA" },
            { "6", "Partners" },
            { "13", "Public" },
            { "11", "Public - Early Bird 1" },
            { "12", "Public - Early Bird 2" },
            { "17", "Special Fields" },
            { "5", "Sponsors" },
            { "3", "Supporting Organisations" },
            { "1", "VIPs" },
            { "7", "Youth" }
        };
        public static readonly Dictionary<string, string> dtDiscountType = new Dictionary<string, string>
        {
            { "Number", "Number" },
            { "Percent", "Percent" }
        };
    }
    
    public static class Email
    {
        public static int SendConfirmation(Guid regId, UnitOfWork unitOfWork)
        {
            int rtn = 0;
            var setting = GeneralHelper.GetSetting(unitOfWork);
            var reg = unitOfWork.RegistrationRepository.GetByID(regId);
            string message = ReadEmailTemplate(setting.RootStoragePhysical + "Confirmation.html");
            message = message.Replace("#serialno#", reg.SerialNo);
            message = message.Replace("#name#", reg.Name);
            message = message.Replace("#banner#", setting.WebUrl + "Images/banner.png");
            message = message.Replace("#designation#", reg.Designation);
            message = message.Replace("#idType#", GeneralHelper.GetIdTypeName(reg.IDType));
            message = message.Replace("#idNumber#", reg.IDNumber);
            message = message.Replace("#mobile#", reg.Mobile);
            message = message.Replace("#email#", "<span style='color:#ffffff'>" + reg.Email + "</span>");
            message = message.Replace("#organisation#", reg.Organisation);
            if(reg.Image == null)
            {
                message = message.Replace("#photoinfo#", "You may present the QR code and your photo I/D in exchange for your event pass.");
            }
            else
            {
                message = message.Replace("#photoinfo#", "Event pass collection will be assisted by FR technology. Scanning of the QR code as an alternate mode.");
            }
            

            AlternateView av2 = AlternateView.CreateAlternateViewFromString(message, null, "text/html");

            LinkedResource linkedresource = null;
            
            linkedresource = new LinkedResource(setting.WebLocation + "Images\\HTXlaunch_banner_FA.jpg", "image/png");
            linkedresource.ContentId = "header";
            av2.LinkedResources.Add(linkedresource);

            MemoryStream memory = null;
            if(reg.Image != null)
            {
                memory = new MemoryStream(reg.Image);
                memory.Write(reg.Image, 0, reg.Image.Length);
                memory.Position = 0;
                linkedresource = new LinkedResource(memory);
                linkedresource.ContentType.MediaType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                linkedresource.ContentId = "photo";
                linkedresource.ContentType.Name = "Photo";
                av2.LinkedResources.Add(linkedresource);
            }
            else
            {
                byte[] imgData = System.IO.File.ReadAllBytes(setting.WebLocation + "Images\\dummy.png");
                memory = new MemoryStream(imgData);
                memory.Write(imgData, 0, imgData.Length);
                memory.Position = 0;
                linkedresource = new LinkedResource(memory);
                linkedresource.ContentType.MediaType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                linkedresource.ContentId = "photo";
                linkedresource.ContentType.Name = "Photo";
                av2.LinkedResources.Add(linkedresource);
            }

            memory = new MemoryStream();
            memory = GeneralHelper.GenerateQrcode(reg.SerialNo, 100, 100);
            linkedresource = new LinkedResource(memory);
            linkedresource.ContentType.MediaType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
            linkedresource.ContentId = "qrcode";
            linkedresource.ContentType.Name = "Qr";

            av2.LinkedResources.Add(linkedresource);
            

            SendMail(reg.Email, setting.EmailSender, setting.EmailSenderName, setting.EmailUsername, setting.EmailPassword, setting.EmailCC, setting.EmailBcc, setting.EmailConfirmationSubject, message, true, null, av2, setting.EmailHost, (int)setting.EmailPort);
            return rtn;
        }
        public static int SentInvite(Guid regId, UnitOfWork unitOfWork)
        {
            int rtn = 0;
            var setting = unitOfWork.SettingRepository.Get().FirstOrDefault();
            var reg = unitOfWork.RegistrationRepository.GetByID(regId);
            reg.StepsAction = RegStepAction.INVT.ToString();
            unitOfWork.RegistrationRepository.Update(reg);
            unitOfWork.Save(); //update flag to email sent

            string regUrl = setting.WebUrl + "Home/Index/" + GeneralHelper.Base64Encode(reg.Id.ToString());

            string message = ReadEmailTemplate(setting.RootStoragePhysical + "Invitation.html");
            message = message.Replace("#name#", reg.Name);
            message = message.Replace("#imglnk#", setting.WebUrl + "Home/TrackEmail/" + GeneralHelper.Base64Encode(reg.Id.ToString()));
            message = message.Replace("#link#", regUrl);
            
            AlternateView av2 = AlternateView.CreateAlternateViewFromString(message, null, "text/html");

            LinkedResource linkedresource = new LinkedResource(setting.WebLocation + "Images\\td1.png", "image/png");
            linkedresource.ContentId = "imgtd1";
            av2.LinkedResources.Add(linkedresource);

            linkedresource = new LinkedResource(setting.WebLocation + "Images\\td2.png", "image/png");
            linkedresource.ContentId = "imgtd2";
            av2.LinkedResources.Add(linkedresource);


            linkedresource = new LinkedResource(setting.WebLocation + "Images\\td3.png", "image/png");
            linkedresource.ContentId = "imgtd3";
            av2.LinkedResources.Add(linkedresource);


            linkedresource = new LinkedResource(setting.WebLocation + "Images\\td4.png", "image/png");
            linkedresource.ContentId = "imgtd4";
            av2.LinkedResources.Add(linkedresource);

            linkedresource = new LinkedResource(setting.WebLocation + "Images\\td5.png", "image/png");
            linkedresource.ContentId = "imgtd5";
            av2.LinkedResources.Add(linkedresource);


            MemoryStream memory = new MemoryStream();
            memory = GeneralHelper.GenerateQrcode(regUrl, 119, 119);
            linkedresource = new LinkedResource(memory);
            linkedresource.ContentType.MediaType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
            linkedresource.ContentId = "qrcode";
            linkedresource.ContentType.Name = "Qr";
            av2.LinkedResources.Add(linkedresource);

            linkedresource = new LinkedResource(setting.WebLocation + "Images\\td6.png", "image/png");
            linkedresource.ContentId = "imgtd6";
            av2.LinkedResources.Add(linkedresource);

            linkedresource = new LinkedResource(setting.WebLocation + "Images\\td7.png", "image/png");
            linkedresource.ContentId = "imgtd7";
            av2.LinkedResources.Add(linkedresource);
            
            SendMail(reg.Email, setting.EmailSender, setting.EmailSenderName, setting.EmailUsername, setting.EmailPassword, setting.EmailCC, setting.EmailBcc, setting.EmailInviteSubject, message, true, null, av2, setting.EmailHost, (int)setting.EmailPort);
            return rtn;
        }

        public static int SentInviteOpenForm(string email, UnitOfWork unitOfWork)
        {
            int rtn = 0;
            var setting = GeneralHelper.GetSetting(unitOfWork);
            

            string regUrl = setting.WebUrl + "Home/Index/tafA95uKbbSDTHdX";

            string message = ReadEmailTemplate(setting.RootStoragePhysical + "InvitationOpenForm.html");
            message = message.Replace("#link#", regUrl);



            AlternateView av2 = AlternateView.CreateAlternateViewFromString(message, null, "text/html");

            LinkedResource linkedresource = new LinkedResource(setting.WebLocation + "Images\\td1.png", "image/png");
            linkedresource.ContentId = "imgtd1";
            av2.LinkedResources.Add(linkedresource);

            linkedresource = new LinkedResource(setting.WebLocation + "Images\\td2-open-form.png", "image/png");
            linkedresource.ContentId = "imgtd2";
            av2.LinkedResources.Add(linkedresource);


            linkedresource = new LinkedResource(setting.WebLocation + "Images\\td3-open-form.png", "image/png");
            linkedresource.ContentId = "imgtd3";
            av2.LinkedResources.Add(linkedresource);


            linkedresource = new LinkedResource(setting.WebLocation + "Images\\td4.png", "image/png");
            linkedresource.ContentId = "imgtd4";
            av2.LinkedResources.Add(linkedresource);

            linkedresource = new LinkedResource(setting.WebLocation + "Images\\td5.png", "image/png");
            linkedresource.ContentId = "imgtd5";
            av2.LinkedResources.Add(linkedresource);


            MemoryStream memory = new MemoryStream();
            memory = GeneralHelper.GenerateQrcode(regUrl, 119, 119);
            linkedresource = new LinkedResource(memory);
            linkedresource.ContentType.MediaType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
            linkedresource.ContentId = "qrcode";
            linkedresource.ContentType.Name = "Qr";
            av2.LinkedResources.Add(linkedresource);

            linkedresource = new LinkedResource(setting.WebLocation + "Images\\td6.png", "image/png");
            linkedresource.ContentId = "imgtd6";
            av2.LinkedResources.Add(linkedresource);

            linkedresource = new LinkedResource(setting.WebLocation + "Images\\td7.png", "image/png");
            linkedresource.ContentId = "imgtd7";
            av2.LinkedResources.Add(linkedresource);

            SendMail(email, setting.EmailSender, setting.EmailSenderName, setting.EmailUsername, setting.EmailPassword, setting.EmailCC, setting.EmailBcc, setting.EmailInviteSubject, message, true, null, av2, setting.EmailHost, (int)setting.EmailPort);
            return rtn;
        }

        public static int SenOTPCOde(Guid id, UnitOfWork unitOfWork)
        {
            int rtn = 0;
            var setting = GeneralHelper.GetSetting(unitOfWork);
            var admin = unitOfWork.AdministratorRepository.GetByID(id);
            
            string message = ReadEmailTemplate(setting.RootStoragePhysical + "OtpTemplate.html");
            message = message.Replace("#code#", admin.OtpCode);
            

            SendMail(admin.Email, setting.EmailSender, setting.EmailSenderName, setting.EmailUsername, setting.EmailPassword, setting.EmailCC, setting.EmailBcc, "HTX OTP", message, true, null, null, setting.EmailHost, (int)setting.EmailPort);
            return rtn;
        }
        public static string ReadEmailTemplate(string filePath)
        {
            // Read the file as one string.
            string _fileContent = "";

            try
            {
                System.IO.StreamReader _file = new System.IO.StreamReader(filePath);
                _fileContent = _file.ReadToEnd();

                _file.Close();
            }
            catch (Exception err)
            {
                _fileContent = err.Message;
            }

            return (_fileContent);
        }
        public static int SendMail(string mailTo, string mailFrom, string mailFromName, string mailUsername, string mailPassword, string mailCC, string mailBcc, string subject, string body, bool IsHTML, AttachmentCollection attachmentCollection, AlternateView alternateview, string host, int port)
        {
            int rtnValue = 0;

            if (mailTo.Trim() == "") return -1;

            MailMessage msg = new MailMessage();
            NetworkCredential networkCredential = null;
            msg.From = new MailAddress(mailFrom, mailFromName);
            try
            {
                //msg.To.Add(new MailAddress(mailTo));

                for (int i = 0; i < mailTo.Split(',').Length; i++)
                    msg.To.Add(new MailAddress(mailTo.Split(',')[i]));
            }
            catch { return -1; }

            if (mailCC != null && mailCC != "")
            {
                for (int i = 0; i < mailCC.Split(',').Length; i++)
                    msg.CC.Add(new MailAddress(mailCC.Split(',')[i]));
            }

            // standard Bcc
            string bcc = "";
            bcc = System.Configuration.ConfigurationSettings.AppSettings["Bcc"];
            if (bcc != null && bcc != "")
            {
                for (int i = 0; i < bcc.Split(',').Length; i++)
                    msg.Bcc.Add(new MailAddress(bcc.Split(',')[i]));
            }

            // Custom Bcc
            if (mailBcc != null && mailBcc != "")
            {
                for (int i = 0; i < mailBcc.Split(',').Length; i++)
                    msg.Bcc.Add(new MailAddress(mailBcc.Split(',')[i]));
            }


            msg.Subject = subject;
            msg.Body = body;
            if (IsHTML == true)
                msg.IsBodyHtml = true;
            else
                msg.IsBodyHtml = false;

            if (alternateview != null)
            {
                msg.AlternateViews.Add(alternateview);
            }

            if (attachmentCollection != null)
            {
                foreach (Attachment attachment in attachmentCollection)
                {
                    msg.Attachments.Add(attachment);
                }
            }

            try
            {
                SmtpClient client = new SmtpClient();
                client.Host = host;
                client.Port = port;
                if(!String.IsNullOrEmpty(mailUsername) && !String.IsNullOrEmpty(mailPassword))
                {
                    networkCredential = new NetworkCredential(mailUsername, mailPassword);
                    client.UseDefaultCredentials = false;
                    client.Credentials = networkCredential;
                }
                //client.Credentials
                client.Send(msg);
            }
            catch (Exception e)
            {
                rtnValue = -1;
            }

            return rtnValue;
        }
    }

    public static class Validate
    {
        public static bool IsValidDateTime(string dateStr, string timeStr)
        {
            try
            {
                var datetime = Convert.ToDateTime(dateStr + " " + timeStr);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public static string ValidStringField(string input, string fieldName, bool isRequired, int? maxLength, string type = null)
        {
            string errorMessage = "";

            if (isRequired && string.IsNullOrEmpty(input))
            {
                return errorMessage = fieldName + " is required.\n";
            }

            if (maxLength != null && !string.IsNullOrEmpty(input) && input.Length > maxLength)
                return errorMessage = fieldName + " length can not more than " + maxLength.ToString() + "\n";

            if (!string.IsNullOrEmpty(type))
            {
                if (type == "Email" && !IsValidEmail(input))
                {
                    return errorMessage = fieldName + " is invalid.\n";
                }

                if (!string.IsNullOrEmpty(input) && type == "Phone")
                {
                    char[] test = input.ToCharArray();

                    foreach (char chr in test)
                    {
                        try
                        {
                            int intTest = 0;
                            if (chr.ToString() != "" && chr.ToString() != "-" && chr.ToString() != "–")
                                intTest = System.Convert.ToInt32(chr.ToString());
                        }
                        catch
                        {
                            return errorMessage = fieldName + "  should be in number only.<br>";
                        }
                    }
                }
            }

            return errorMessage;
        }
        public static bool IsNRICValid(string NRIC)
        {
            bool ret = true;

            if (NRIC.Length != 9) return false;

            // check that the 1st 7 are numbers
            try { long test = long.Parse(NRIC.Substring(1, 7)); }
            catch { ret = false; }

            // multiply the 7 digits with the weight constant
            if (ret)
            {
                int m = 1;
                m = int.Parse(NRIC.Substring(1, 1)) * 2;
                m += int.Parse(NRIC.Substring(2, 1)) * 7;
                m += int.Parse(NRIC.Substring(3, 1)) * 6;
                m += int.Parse(NRIC.Substring(4, 1)) * 5;
                m += int.Parse(NRIC.Substring(5, 1)) * 4;
                m += int.Parse(NRIC.Substring(6, 1)) * 3;
                m += int.Parse(NRIC.Substring(7, 1)) * 2;

                // get the remainder after div by 11
                int remainder = m % 11;

                // get the check digit
                //int check = 11 - remainder;

                // convert the alphabet
                string alpha = "";
                if (NRIC.Substring(0, 1).ToUpper() == "S")
                {
                    switch (remainder)
                    {
                        case 10:
                            alpha = "A";
                            break;
                        case 9:
                            alpha = "B";
                            break;
                        case 8:
                            alpha = "C";
                            break;
                        case 7:
                            alpha = "D";
                            break;
                        case 6:
                            alpha = "E";
                            break;
                        case 5:
                            alpha = "F";
                            break;
                        case 4:
                            alpha = "G";
                            break;
                        case 3:
                            alpha = "H";
                            break;
                        case 2:
                            alpha = "I";
                            break;
                        case 1:
                            alpha = "Z";
                            break;
                        case 0:
                            alpha = "J";
                            break;
                    }
                }

                if (NRIC.Substring(0, 1).ToUpper() == "T")
                {
                    switch (remainder)
                    {
                        case 10:
                            alpha = "H";
                            break;
                        case 9:
                            alpha = "I";
                            break;
                        case 8:
                            alpha = "Z";
                            break;
                        case 7:
                            alpha = "J";
                            break;
                        case 6:
                            alpha = "A";
                            break;
                        case 5:
                            alpha = "B";
                            break;
                        case 4:
                            alpha = "C";
                            break;
                        case 3:
                            alpha = "D";
                            break;
                        case 2:
                            alpha = "E";
                            break;
                        case 1:
                            alpha = "F";
                            break;
                        case 0:
                            alpha = "G";
                            break;
                    }
                }

                if (NRIC.Substring(0, 1).ToUpper() == "F")
                {
                    switch (remainder)
                    {
                        case 10:
                            alpha = "K";
                            break;
                        case 9:
                            alpha = "L";
                            break;
                        case 8:
                            alpha = "M";
                            break;
                        case 7:
                            alpha = "N";
                            break;
                        case 6:
                            alpha = "P";
                            break;
                        case 5:
                            alpha = "Q";
                            break;
                        case 4:
                            alpha = "R";
                            break;
                        case 3:
                            alpha = "T";
                            break;
                        case 2:
                            alpha = "U";
                            break;
                        case 1:
                            alpha = "W";
                            break;
                        case 0:
                            alpha = "X";
                            break;
                    }
                }

                if (NRIC.Substring(0, 1).ToUpper() == "G")
                {
                    switch (remainder)
                    {
                        case 10:
                            alpha = "T";
                            break;
                        case 9:
                            alpha = "U";
                            break;
                        case 8:
                            alpha = "W";
                            break;
                        case 7:
                            alpha = "X";
                            break;
                        case 6:
                            alpha = "K";
                            break;
                        case 5:
                            alpha = "L";
                            break;
                        case 4:
                            alpha = "M";
                            break;
                        case 3:
                            alpha = "N";
                            break;
                        case 2:
                            alpha = "P";
                            break;
                        case 1:
                            alpha = "Q";
                            break;
                        case 0:
                            alpha = "R";
                            break;
                    }
                }

                // compare
                if (alpha.ToUpper() != NRIC.Substring(8, 1).ToUpper())
                    ret = false;
            }

            return ret;
        }

        public static bool IsRegFormOpenForm(string param)
        {
            return (param == "tafA95uKbbSDTHdX");
        }
    }


    public static class GeneralHelper
    {
        public static Setting GetSetting(UnitOfWork unitOfWork)
        {
            return unitOfWork.SettingRepository.Get().FirstOrDefault();
        }

        public static string GetEmailDomain(string email)
        {
            if (string.IsNullOrEmpty(email) || email.IndexOf("@") == -1) return "";

            return email.Substring(email.IndexOf("@") + 1, email.Length - email.IndexOf("@") - 1);
        }

        // Base64
        public static string Base64Encode(string plainText)
        {
            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return System.Convert.ToBase64String(plainTextBytes);
            }
            catch
            {
                return "";
            }
        }
        public static string Base64Decode(string encodedString)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(encodedString);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch
            {
                return "";
            }
        }

        public static string GetIdTypeName(string idCode)
        {
            switch (idCode)
            {
                case "1":
                    return "NRIC";
                    break;
                case "2":
                    return "Pasport No";
                    break;
            }
            return "";
        }

        public static string GetRegStatus(string status)
        {
            foreach(var item in VarRef.dtRegStatus)
            {
                if(item.Key == status)
                {
                    return item.Value;
                }
            }

            return "";
        }
        public static string GetRegStepStatus(string status)
        {
            switch (status)
            {
                case "DFT":
                    return "Draft";
                    break;
                case "INVT":
                    return "Invited";
                    break;
                case "READ":
                    return "Email Read";
                    break;
                case "CLICK":
                    return "Clicked";
                    break;
                case "CPLT":
                    return "Completed";
                    break;
            }
            
            return "";
        }

        // Hash
        private static string salt = "IPTech2019";
        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
        public static string HashString(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(input + salt))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        // IPTech Lib: String Encryption/Decryption
        public static string StringEncryption(string normalText)
        {
            if (normalText == null)
            {
                return "";
            }
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(normalText);
            return System.Convert.ToBase64String(bytes);
        }
        public static string StringDecryption(string encryptedText)
        {
            if (encryptedText == null)
            {
                return "";
            }
            byte[] bytes = System.Convert.FromBase64String(encryptedText);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }


        public static string GenerateOTP()
        {
            return Guid.NewGuid().ToString().Substring(0, 6);
        }
        public static string GetHashedString(string input)
        {
            var hash = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }
        public static string GenerateSerial(UnitOfWork unitOfWork)
        {
            var setting = GeneralHelper.GetSetting(unitOfWork);

            string serialformat = setting.SerialFormat;
            string number = setting.RunningNo;
            string newnumber = (int.Parse(number) + 1).ToString().PadLeft(5, '0');
            lock (unitOfWork)
            {
                setting.RunningNo = newnumber;

                unitOfWork.SettingRepository.Update(setting);
                unitOfWork.Save();
            }

            return setting.SerialFormat + number;
        }
        public static string GetEventStatus(string conferenceData, string roundTable1, string roundTable2, string roundTable3, Guid eventId)
        {
            string result = "";
            string idStr = eventId.ToString().ToLower();
            if (conferenceData.Trim().ToLower().Contains(idStr) ||
                roundTable1.Trim().ToLower().Contains(idStr) ||
                roundTable2.Trim().ToLower().Contains(idStr) ||
                roundTable3.Trim().ToLower().Contains(idStr))
                result = "checked";

            return result;
        }
        public static string GetCheckboxStatus(string inputValue, string chkValue)
        {
            string result = "";

            if (inputValue == chkValue)
                result = "checked";

            return result;
        }
        public static string GetDateString(DateTime? datetime)
        {
            return datetime != null ? datetime.Value.Date.ToString("yyyy-MM-dd") : "";
        }
        public static string GetTimeString(DateTime? datetime)
        {
            return datetime != null ?
                datetime.Value.TimeOfDay.Hours.ToString().PadLeft(2, '0') + ":" +
                datetime.Value.TimeOfDay.Minutes.ToString().PadLeft(2, '0') + ":" +
                datetime.Value.TimeOfDay.Seconds.ToString().PadLeft(2, '0')
                :
                "";
        }

        public static string GetDateString2(DateTime? datetime)
        {
            return datetime != null ? datetime.Value.Date.ToString("dd-MM-yyyy") : "";
        }
        public static string GetTimeString2(DateTime? datetime)
        {
            return datetime != null ?
                datetime.Value.TimeOfDay.Hours.ToString().PadLeft(2, '0') + ":" +
                datetime.Value.TimeOfDay.Minutes.ToString().PadLeft(2, '0')
                :
                "";
        }


        public static bool IsShowingFlightAcc(Models.DB.Registration regItem)
        {
            bool result = true;

            // TODO: check if reg item progress contains "Flight & Acc" step, this may related to promo code

            return result;
        }
        public static string EventSelectedStatus(string selectedEventIdList, string eventCheckingWith)
        {
            try
            {
                if (selectedEventIdList.Contains(eventCheckingWith))
                {
                    return "checked";
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public static string GetSelectedEventList(string conferenceData)
        {
            string result = "";

            return result;
        }
        
        public static string GenerateURlPromoCode(UnitOfWork unitOfWork, Guid promocodeId, string emailRegistration)
        {
            string urlResult = "";
            Setting setting = unitOfWork.SettingRepository.Get().SingleOrDefault();
            if (setting != null)
            {
                string firstPara = GeneralHelper.Base64Encode(promocodeId.ToString()) + "|" + GeneralHelper.Base64Encode(emailRegistration);
                urlResult = setting.WebUrl + "/Registration/login?p=" + firstPara + "&h=" + GeneralHelper.GetHashedString(firstPara);
            }

            return urlResult;
        }
	    public class DiscountInfo
	    {
	        public DiscountInfo() { }
	        public DiscountInfo(string title, decimal price)
	        {
	            Title = title;
	            Price = price;
	        }
	        public string Title { get; set; }
	        public decimal Price { get; set; }
	    }

        public static System.IO.MemoryStream GetQRCodeImage(string serialNo)
        {
            //Code128BarcodeDraw bdf = BarcodeDrawFactory.Code128WithChecksum;
            //System.Drawing.Image img = bdf.Draw(serialNo, 55, 2);

            BarcodeSymbology s = BarcodeSymbology.Code128;
            BarcodeDraw drawObject = BarcodeDrawFactory.CodeQr;
            var metrics = drawObject.GetDefaultMetrics(50);
            metrics.Scale = 5;
            var img = drawObject.Draw(serialNo, metrics);

            ImageConverter ic = new ImageConverter();

            Byte[] ba = (Byte[])ic.ConvertTo(img, typeof(Byte[]));

            MemoryStream memoryStream = new MemoryStream(ba);
            memoryStream.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }

        public static MemoryStream GenerateUserPhoto(byte[] image)
        {
            MemoryStream memory = new MemoryStream(image);
            memory.Write(image, 0, image.Length);
            memory.Position = 0;
            return memory;
        }
        public static System.IO.MemoryStream GenerateQrcode(string textContent, int height = 100, int width = 100)
        {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

            // instantiate a writer object
            ZXing.BarcodeWriter barcodeWriter = new ZXing.BarcodeWriter();

            // set the barcode format
            ZXing.QrCode.QrCodeEncodingOptions qrCodeEncodingOptions = new ZXing.QrCode.QrCodeEncodingOptions();
            qrCodeEncodingOptions.Margin = 1;
            qrCodeEncodingOptions.Height = height;
            qrCodeEncodingOptions.Width = width;
            barcodeWriter.Format = ZXing.BarcodeFormat.QR_CODE;
            barcodeWriter.Options = qrCodeEncodingOptions;

            // write text and generate a 2-D barcode as a bitmap
            barcodeWriter.Write(textContent).Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            memoryStream.Flush();
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);

            return memoryStream;
        }
        public static MemoryStream GenerateSingleBadge(Guid Id, UnitOfWork unitOfWork)
        {

            Setting setting = GetSetting(unitOfWork);
            Registration reg = unitOfWork.RegistrationRepository.GetByID(Id);

            string pdfPath = setting.WebLocation + "Storage\\Pdf\\Badge.pdf";

            PdfReader pdfReader = null;
            MemoryStream memoryStreamPdfStamper = null;
            PdfStamper pdfStamper = null;
            AcroFields pdfFormFields = null;

            pdfReader = new PdfReader(pdfPath);

            memoryStreamPdfStamper = new MemoryStream();
            pdfStamper = new PdfStamper(pdfReader, memoryStreamPdfStamper);
            pdfFormFields = pdfStamper.AcroFields;
            
            pdfFormFields.SetField("name", reg.Name);


            // Barcode
            MemoryStream memoryStream = GenerateQrcode(reg.SerialNo, 119, 119);
            iTextSharp.text.Image img = null;
            PdfContentByte pdfContentByte = null;

            img = iTextSharp.text.Image.GetInstance(memoryStream);
            img.ScaleToFit(190, 30);
            pdfContentByte = pdfStamper.GetOverContent(1);
            img.SetAbsolutePosition(95f, 59f);
            
            pdfContentByte.AddImage(img);

            memoryStream = new MemoryStream(reg.Image);
            img = iTextSharp.text.Image.GetInstance(memoryStream);
            img.ScaleToFit(290, 120);
            pdfContentByte = pdfStamper.GetOverContent(1);
            img.SetAbsolutePosition(70f, 140f);

            pdfContentByte.AddImage(img);

            pdfStamper.FormFlattening = true;
            pdfStamper.Writer.CloseStream = false;
            pdfStamper.Close();

            memoryStreamPdfStamper.Flush();
            memoryStreamPdfStamper.Seek(0, SeekOrigin.Begin);

            //MemoryStream docstream = CreatePdfDoc(memoryStreamPdfStamper);
            //memoryStreamPdfStamper.Dispose();

            //docstream.Position = 0;
            //return docstream;

            //memoryStreamPdfStamper.Position = 0;
            return memoryStreamPdfStamper;

        }

        public static MemoryStream GenerateAllBadge(List<Registration> regList, UnitOfWork unitOfWork)
        {
            List<MemoryStream> memoryStreamPdfStamperList = new List<MemoryStream>();
            MemoryStream memoryStreamPdfStamper = null;

            foreach (Registration r in regList)
            {
                memoryStreamPdfStamper = GenerateSingleBadge(r.Id, unitOfWork);
                memoryStreamPdfStamperList.Add(memoryStreamPdfStamper);
            }


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////

            MemoryStream docstream = CreatePdfDoc(memoryStreamPdfStamperList);

            foreach (MemoryStream memoryStreamPdfStamper1 in memoryStreamPdfStamperList)
                memoryStreamPdfStamper1.Dispose();

            docstream.Position = 0;
            return docstream;
        }
        public static MemoryStream CreatePdfDoc(List<MemoryStream> memoryStreamPdfStamperList)
        {
            iTextSharp.text.Document document = new iTextSharp.text.Document();

            if (memoryStreamPdfStamperList.Count > 0)
            {
                document = new iTextSharp.text.Document(iTextSharp.text.PageSize.B8);
            }

            MemoryStream memoryStreamDocument = new MemoryStream();
            PdfWriter pdfWriter = PdfWriter.GetInstance(document, memoryStreamDocument);
            document.Open();
            pdfWriter.CloseStream = false;

            foreach (MemoryStream memoryStreamPdfStamper in memoryStreamPdfStamperList)
            {
                PdfContentByte pdfContentByte = pdfWriter.DirectContent;
                document.NewPage();

                PdfImportedPage pdfImportedPage = pdfWriter.GetImportedPage(new PdfReader(memoryStreamPdfStamper.GetBuffer()), 1);

                pdfContentByte.AddTemplate(pdfImportedPage, 0, 0);
                //const float posX = 20f;
                //const float posY = 30f;
                //pdfContentByte.AddTemplate(pdfImportedPage, posX, posY);
                memoryStreamPdfStamper.Dispose();
            }

            document.Close();

            return memoryStreamDocument;
        }

        public static Image ByteArrayToImage(byte[] _imageArray)
        {
            MemoryStream ms = new MemoryStream(_imageArray);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }

    public class Logging
    {
        public static void CreateAuditLog(Guid regId, string adminId, string action, string module, UnitOfWork unitOfWork, Registration regObj = null, string subModule = "")
        {
            JavaScriptSerializer serilize = new JavaScriptSerializer();
            AuditLog audit = new AuditLog();
            audit.Id = Guid.NewGuid();
            audit.RegId = regId;
            audit.AdminId = adminId;
            audit.Action = action;
            audit.Module = module;
            if(regObj != null)
            {
                regObj.Image = null;

                string jsonString = serilize.Serialize(regObj);
                audit.DataChange = jsonString;
            }
            audit.SubModule = subModule;
            audit.DateAction = DateTime.Now;
            unitOfWork.AuditLog.Insert(audit);
            unitOfWork.Save();
        }
    }
}