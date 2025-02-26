# AzureStorageApp

## Project Purpose
This is a basic CRUD application using Azure Storage. It supports Read, Upsert, and Delete operations through a .NET 6 REST API. The project utilizes Blob Storage for file management, Table Storage for structured data, and Queue Storage for messaging. This implementation serves as a practice for working with Azure Storage in a RESTful architecture.

## Contributors
Darshana Wijesinghe

## App Features
- Supports Read, Upsert, and Delete operations with Azure Table Storage.
- Supports Blob Storage for file management.
- Supports Queue Storage for messaging.

## Packages
- Azure.Data.Tables
- Azure.Storage.Blobs
- Azure.Storage.Queues
- Newtonsoft.Json

## Usage
```json5
// Creates a new task

POST /api/storage/create-record

// Request Body

{
  "TaskName": "The new task for test.",
  "Assignee": "Darshana",
  "FileName": "flow.jpg",
  "Base64File": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAYABgAA...",
  "Deadline": "2025-02-26"
}
```
```json5
// Gets a specific task

GET /api/storage/get-task?partitionKey={partitionKey}&rowKey={rowKey}&fileName={fileName}
```
```json5
// Gets all tasks

GET /api/storage/get-tasks
```
```json5
// Deletes an existing task

DELETE /api/storage/delete-task?partitionKey={partitionKey}&rowKey={rowKey}&fileName={fileName}
```
## Support
Darshana Wijesinghe  
Email address - [dar.mail.work@gmail.com](mailto:dar.mail.work@gmail.com)  
Linkedin - [darwijesinghe](https://www.linkedin.com/in/darwijesinghe/)  
GitHub - [darwijesinghe](https://github.com/darwijesinghe)

## License
This project is licensed under the terms of the **MIT** license.