using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XmlXsdValidator
{
  /// <summary>
  /// Author: Pálmi Skowronski
  /// Date: 2013-07-04
  /// 
  /// Validates a xml file with a xsd file.
  /// Based on code from Bangla Gopal Surya
  /// Ref: http://www.codeproject.com/Articles/276945/XML-Validation-with-XSD-along-with-Custom-Exceptio
  /// </summary>
  class Program
  {
    private static int _issueCounter;
    private static List<string> _validationComments;

    static void Main(string[] args)
    {
      string content = "content.xml";
      string validator = "validator.xsd";
      _validationComments = new List<string>();

      if (args.Length > 0 && args[0].Length > 4)
      { content = args[0].ToString(); }

      if (args.Length > 1 && args[1].Length > 4)
      { validator = args[1].ToString(); }

      Console.WriteLine("Validating {0} with {1}", content, validator);

      ValidateXmlWithXsd(content, validator);

      Console.WriteLine("Press any key to continue...");
      Console.ReadLine();
    }

    #region production
    private static bool ValidateXmlWithXsd(string xmlPath, string xsdPath)
    {
      bool isValid = false;
      try
      {
        // Declare local objects
        XmlReaderSettings rearderSettings = new XmlReaderSettings();
        rearderSettings.ValidationType = ValidationType.Schema;
        rearderSettings.ValidationFlags |= 
          XmlSchemaValidationFlags.ProcessSchemaLocation |
          XmlSchemaValidationFlags.ReportValidationWarnings |
          XmlSchemaValidationFlags.ProcessIdentityConstraints |
          XmlSchemaValidationFlags.AllowXmlAttributes;

        // Event Handler for handling exception
        // This will be called whenever any mismatch between XML & XSD occurs. 
        // Collects validation information which will be reported when all of xml has been read
        rearderSettings.ValidationEventHandler += new ValidationEventHandler(XmlValidationEventHandler);
        rearderSettings.Schemas.Add(null, XmlReader.Create(xsdPath));

        // Reading xml
        using (XmlReader xmlValidatingReader = XmlReader.Create(xmlPath, rearderSettings))
        { while (xmlValidatingReader.Read()) { } }

        // Set return validation status
        isValid = _issueCounter == 0;
      }
      catch (Exception error)
      {
        isValid = false;
        Console.WriteLine("Exception {0}", error.Message);
      }

      ValidationReport();
      return isValid;
    }

    private static void XmlValidationEventHandler(object sender, ValidationEventArgs e)
    {
      _issueCounter++;
      _validationComments.Add(string.Format("{0} @ line {1} position {2}: {3} \r\n",
        (e.Severity == XmlSeverityType.Error ? "ERROR" : "WARNING"),
        e.Exception.LineNumber,
        e.Exception.LinePosition,
        e.Message));
    }

    private static void ValidationReport()
    {
      Console.WriteLine("|=======================================|");
      Console.WriteLine("Xml {0}", _issueCounter > 0 ? "is not valid" : "is valid");
      if (_issueCounter > 0)
      {
        Console.WriteLine("{0} Warnings or Errors",_issueCounter);
        foreach (var comment in _validationComments) { Console.WriteLine(comment); }
      }
      Console.WriteLine("|=======================================|");
    }
    #endregion production

  }
}
