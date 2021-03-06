﻿using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LCSStatementParser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        //Button click listener to beign conversion
        //Most operational logic stems from here
        private void button1_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox1.Text))
            {
                String filename = System.IO.Path.GetFileNameWithoutExtension(textBox1.Text);
                List<String> stringList = ExtractTextFromPdf(textBox1.Text);
                stringList = separateLineBreaks(stringList);

                stringList = cleanList(stringList);
                stringList = removeComma(stringList);

                foreach (String s in stringList)
                {
                    Console.WriteLine(s);
                }

                stringList = convertToCsv(stringList);


                try
                {
                    File.WriteAllLines("out.csv", stringList);
                }
                catch (Exception x)
                {
                    MessageBox.Show("File already open!");
                }
                Process.Start("out.csv");
            }
            else
            {
                MessageBox.Show("File Does Not Exist My Friend!");

            }
        }

        public static List<string> ExtractTextFromPdf(String path)
        {
            List<String> stringList = new List<String>();

            using (PdfReader reader = new PdfReader(path))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    stringList.Add(PdfTextExtractor.GetTextFromPage(reader, i));
                }
                return stringList;
            }
        }

        //Extracts Dates from files and adds 2017 to the end 
        //TODO not static. 
        private static String extractDatesFromFile1(String input)
        {

            String s = "";

            s += input[3];
            s += input[4];

            s += ".17";

            return s;

        }


        private static String extractDateFromFile2(String input)
        {
            String s = "";

            s += input[0];
            s += input[1];
            s += input[2];
            s += input[3];
            s += input[4];

            return s;
        }



        //Separates the string list w.r.t breaks
        public static List<String> separateLineBreaks(List<String> stringList)
        {
            List<String> newList = new List<String>();
            foreach (String s in stringList)
            {
                string[] lines = s.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                foreach (string st in lines)
                {
                    newList.Add(st);
                }
            }

            return newList;
        }

        //Perform a cleanup on the list
        public static List<String> cleanList(List<String> stringList)
        {
            List<String> cleanList = new List<String>();

            for (int i = 0; i < stringList.Count - 1; i++)
            {
                if (Char.IsNumber(stringList[i][0]) && (stringList[i][2] == '.'))
                {
                    cleanList.Add(stringList[i]);
                }
            }
            return cleanList;
        }
        //Replace all commas in the string list to fullstops
        public static List<String> removeComma(List<String> stringList)
        {
            List<String> cleanList = new List<String>();

            for (int i = 1; i < stringList.Count - 1; i++)
            {
                cleanList.Add(stringList[i].Replace(",", "."));
            }

            return cleanList;
        }

        //Convert to CSV using comma as delimeter. Also performs some formatting on the dates
        //TODO: Consider extracting to two separate methods 
        public static List<String> convertToCsv(List<String> stringList)
        {
            List<String> csvList = new List<String>();

            foreach (String s in stringList)
            {
                int i = s.IndexOf(extractDatesFromFile1(s));

                String s2 = s.Insert(i + 5, ", ,");

                s2 += ", , ,";

                s2 += (extractDateFromFile2(s) + ".17").Replace(".", "/");
                csvList.Add(s2);
            }
            return csvList;
        }
        //Listener for the button
        //Opens a file dialog to open the file
        private void button2_Click(object sender, EventArgs e)
        {

            openFileDialog1.Filter = "PDF Files (.pdf)|*.pdf|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = true;
            // Process input if the user clicked OK.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }

        }
    }
}
