# User-Event-Module 
This module enables users to 
  - List a single event, 
  - List all the events, 
  - List the events created by the current user
  - List the events participated by the current user
  - Create an event 
  - Update an event that is created by the user
  - Delete an event that is created by the user
  - Register for an event that is created by other users
  - Send invitations to other users of event that is created by the inviter user
  - List the received intivations 
  - List the sent invitations
  - Accept to participate an invited event.

# Project Structure
  ![event-app-structure-1](https://user-images.githubusercontent.com/106915107/228821415-7b3820ec-3d6c-4662-b60d-e63f8a6bb07e.png)

# How to run the project in your local  
   

### 1- Clone the repository
```
 git clone https://github.com/onrdr/User-Event-Module
```

### 2- Navigate to the API Directory
```
 cd User-Event-Module
```

### 3- Build the docker image
(If you get docker daemon is not running error, Please make sure that the docker is running on your device.)
```
 docker build -f API/Dockerfile  -t onur-user-events-api .
```

### 4- Start the docker container
```
 docker run -it --rm -p 5000:8090 onur-user-events-api
```

# Notes
### The Url should be in this format : localhost:5000/swagger/index.html

### The API uses SQLite as its database, so no additional setup is required for the database.

### You should now be able to run the project on your local machine now.  

 
 
