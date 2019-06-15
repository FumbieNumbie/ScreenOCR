﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Tesseract;
using System.Diagnostics;
using System.IO;

namespace ScreenOCR
{


	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private string folderPath = System.IO.Path.Combine(Environment.GetFolderPath(
  Environment.SpecialFolder.ApplicationData), @"OCR\temp");
		private string imagePath = System.IO.Path.Combine(Environment.GetFolderPath(
  Environment.SpecialFolder.ApplicationData), @"OCR\temp") + "\\img.png";

		public MainWindow()
		{
			InitializeComponent();
			if (Clipboard.ContainsImage())
			{
				BitmapSource bitmapSource = Clipboard.GetImage();

				image.Source = bitmapSource;
				EncodeToPNG(bitmapSource);
				Debug.WriteLine("test1");
				DoOCR();
			}
			
		}
		private void DoOCR()
		{

			try
			{
				var logger = new FormattedConsoleLogger();
				var resultPrinter = new ResultPrinter(logger);
				using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
				{
					//using (var img = ClipboardImgToBitmap())
					using (var img = Pix.LoadFromFile(imagePath))
					{
						using (logger.Begin("Process image"))
						{
							var i = 1;
							using (var page = engine.Process(img))
							{
								var text = page.GetText();
								logger.Log("Text: {0}", text);
								logger.Log("Mean confidence: {0}", page.GetMeanConfidence());
								outputBlock.Text = text;

								using (var iter = page.GetIterator())
								{
									iter.Begin();
									do
									{
										if (i % 2 == 0)
										{
											using (logger.Begin("Line {0}", i))
											{
												do
												{
													using (logger.Begin("Word Iteration"))
													{
														if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
														{
															logger.Log("New block");
														}
														if (iter.IsAtBeginningOf(PageIteratorLevel.Para))
														{
															logger.Log("New paragraph");
														}
														if (iter.IsAtBeginningOf(PageIteratorLevel.TextLine))
														{
															logger.Log("New line");
														}
														logger.Log("word: " + iter.GetText(PageIteratorLevel.Word));
													}
												} while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));
											}
										}
										i++;
									} while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.Indent();
				Trace.TraceError(e.ToString());
				Debug.WriteLine("Unexpected Error: " + e.Message);
				Debug.WriteLine("Details: ");
				Debug.WriteLine(e.ToString());
			}
		}


		private void EncodeToPNG(BitmapSource bitmapSource)
		{
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			using (FileStream stream = new FileStream(imagePath, FileMode.Create))
			{
				PngBitmapEncoder encoder = new PngBitmapEncoder();
				TextBlock myTextBlock = new TextBlock();
				myTextBlock.Text = "Codec Author is: " + encoder.CodecInfo.Author.ToString();
				encoder.Interlace = PngInterlaceOption.On;
				encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
				encoder.Save(stream);
			}
		}

		private class ResultPrinter
		{
			readonly FormattedConsoleLogger logger;

			public ResultPrinter(FormattedConsoleLogger logger)
			{
				this.logger = logger;
			}

			public void Print(ResultIterator iter)
			{
				logger.Log("Is beginning of block: {0}", iter.IsAtBeginningOf(PageIteratorLevel.Block));
				logger.Log("Is beginning of para: {0}", iter.IsAtBeginningOf(PageIteratorLevel.Para));
				logger.Log("Is beginning of text line: {0}", iter.IsAtBeginningOf(PageIteratorLevel.TextLine));
				logger.Log("Is beginning of word: {0}", iter.IsAtBeginningOf(PageIteratorLevel.Word));
				logger.Log("Is beginning of symbol: {0}", iter.IsAtBeginningOf(PageIteratorLevel.Symbol));

				logger.Log("Block text: \"{0}\"", iter.GetText(PageIteratorLevel.Block));
				logger.Log("Para text: \"{0}\"", iter.GetText(PageIteratorLevel.Para));
				logger.Log("TextLine text: \"{0}\"", iter.GetText(PageIteratorLevel.TextLine));
				logger.Log("Word text: \"{0}\"", iter.GetText(PageIteratorLevel.Word));
				logger.Log("Symbol text: \"{0}\"", iter.GetText(PageIteratorLevel.Symbol));
			}
		}

		private void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			DoOCR();
		}
	}
}