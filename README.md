# 🧾 AI-Powered Bill Organizer

An intelligent web application that automatically extracts and organizes bill information from receipt images using Google Gemini AI.
## ✨ Features

- AI-powered OCR using Google Gemini 2.0 Flash
- Automatic extraction of merchant, date, amount, category, and items
- PostgreSQL database for persistent storage
- Good Looking web interface
- Real-time spending statistics
- Easy bill management

## 🛠️ Tech Stack

**Backend:** ASP.NET Core 8, Entity Framework Core  
**Database:** PostgreSQL 16  
**AI:** Google Gemini 2.0 Flash API  
**Frontend:** HTML, CSS, JavaScript

##What does Each Folder Do
- Controllers - Holds BillsController which handles all incoming HTTP requests (GET, POST, DELETE) from the frontend
- Data - Contains AppDbContext which acts as the bridge between your C# code and PostgreSQL database
- Models - Contains Bill.cs and BillItem.cs classes that define the structure of your data objects
- Services - Houses GeminiService.cs which handles all AI integration logic for extracting data from bill images
- Uploads - Stores all uploaded bill images that users submit through the frontend(its just an empty folder that just stores images bills)
(easy for the AI to take the binary img and process it using base64 encoding, finally returns a json response back to us from google server)

## 📋 Prerequisites

- .NET SDK 8.0+
- PostgreSQL 16+ (because it has LongTermService)
- Google Gemini API Key ([Get it free](https://ai.google.dev/))

By - Amit Kumar
