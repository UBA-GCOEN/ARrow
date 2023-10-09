## /user

- POST: /signup  
   Request

  - **body**

    ```bash
    {
        "email": "youremail@gmail.com",
        "password": "pass@1234",
        "confirmPassword": "pass@1234"
    }
    ```

  Response

  - **body**
    ```bash
    {
        "success": true,
        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InlvdXJlbWFpbEBnbWFpbC5jb20iLCJpZCI6IjY1MjJiZDYxMjI3NThiZTViNmZkZGY4YiIsImlhdCI6MTY5Njc3NTUyMSwiZXhwIjoxNjk3MzgwMzIxfQ.Qw-0eCZ5-KN2zqnj5AKSzLnqsWaUOL74e835wiFEUnw",
        "msg": "User added and logged in successfully"
    }
    ```

- POST: /signin  
   Request

  - **body**

    ```bash
    {
        "email": "youremail@gmail.com",
        "password": "pass@1234",
    }
    ```

  Response

  - **body**
    ```bash
    {
        "success": true,
        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InlvdXJlbWFpbEBnbWFpbC5jb20iLCJpZCI6IjY1MjJiZDYxMjI3NThiZTViNmZkZGY4YiIsImlhdCI6MTY5Njc3NTk1MCwiZXhwIjoxNjk3MzgwNzUwfQ.xG2iKXiISe_Ibknw5aXrIPanCfldDdp2B79D03sdBUs",
        "_id": "6522bd6122758be5b6fddf8b",
        "isOnboarded": false,
        "msg": "User is logged in successfully"
    }
    ```

## GET /logout

Response

- **body**
  ```bash
  Logged out successfully
  ```

## GET /getDeletePage

Response

- **body**
  ```bash
  {
      "name": "test name",
      "email": "email@gmail.com"
  }
  ```

## DELETE /deleteuser

Response

- **body**
  ```bash
  User deleted successffuly
  ```

## POST /sendEmail

Request

- **body**

  ```bash
  {
      "email": "email@gmail.com"
  }
  ```

Response

- **body**
  ```bash
  reset link sent
  ```

## POST /updatePassword

Request

- **body**

  ```bash
  {
      "password": "Naresh@12345",
      "confirmPassword": "Naresh@12345"
  }
  ```

Response

- **body**
  ```bash
  password changed successfully
  ```

## POST /changePassword

Request

- **body**

  ```bash
  {
      "oldPassword": "Naresh@123",
      "newPassword": "Naresh@1234",
      "confirmNewPassword": "Naresh@1234"
  }
  ```

Response

- **body**
  ```bash
  password changed successfully
  ```

## /profile

- POST: /update  
   Request

  - **body**

    ```bash
    {
        "name": "Naresh Test",
        "email": "chandup@gmail.com",
        "password": "Naresh@123",
        "confirmPassword": "Naresh@123",
        "branch": "test69",
        "enrollNo": 123654,
        "subjects": "chema",
        "designation": "design",
        "bio": "test",
        "education": "test",
        "intrest": "test",
        "mobile": 234
    }
    ```

  Response

  - **body**
    ```bash
    {
        "success": true,
        "msg": "user faculty updated successfully",
        "user": {
            "_id": "651965a057de81f757d51e72",
            "email": "chandup@gmail.com",
            "password": "$2b$12$13TesQmIeEuRULv4DIJ0B.w7e2T5tX74TFD3cs3DILFR2yD5t15GO",
            "role": "faculty",
            "isOnboarded": false,
            "__v": 0,
            "bio": "test",
            "branch": "test69",
            "designation": "design",
            "education": "test",
            "intrest": "test",
            "mobile": 234,
            "name": "Naresh Test",
            "subjects": "chema",
            "enrollNo": 123654
        }
    }
    ```

## /event

- POST: /createEvent
  Request

  - **body**

    ```bash
    {
        "title": "test5",
        "description": "tada5",
        "eventCoordinator": "tatascy5",
        "time": "kaya malum 5",
        "venue": "dekh le 5",
        "guest": "ek hi hai 5"
    }
    ```

  Response

  - **body**
    ```bash
    {
    "msg": "event created successfully",
    "_id": "652354881610e7f4e7934d2f",
    "event": {
        "title": "test5",
        "description": "tada5",
        "organizerRole": "faculty",
        "organizerEmail": "youremail@gmail.com",
        "organizerName": "test name",
        "status": "upcoming",
        "eventCoordinator": "tatascy5",
        "time": "kaya malum 5",
        "venue": "dekh le 5",
        "guest": "ek hi hai 5",
        "_id": "652354881610e7f4e7934d2f",
        "__v": 0
    }
    }
    ```

- PUT: /updateEvent
  Request

  - **body**

    ```bash
    {
        "_id": "651573a9edb0cafdfaa0c261",
        "title": "testing done",
        "description": "tada tada tada tada",
        "eventCoordinator": "tatascy",
        "time": "kaya malum",
        "venue": "dekh le",
        "guest": "ek hi hai"
    }
    ```

  Response

  - **body**
    ```bash
    event udated successfully
    ```

- GET: /getSpecificEvent
  Request

  - **body**

    ```bash
    {
        "_id": "651573a9edb0cafdfaa0c261",
    }
    ```

  Response

  - **body**
    ```bash
    {
        "_id": "651573a9edb0cafdfaa0c261",
        "title": "testing done",
        "description": "tada tada tada tada",
        "organizerRole": "faculty",
        "organizerEmail": "youremail@gmail.com",
        "organizerName": "test name",
        "status": "upcoming",
        "eventCoordinator": "tatascy",
        "time": "kaya malum",
        "venue": "dekh le",
        "guest": "ek hi hai",
        "__v": 0
    }
    ```

- GET: /getAllEvent

  Response

  - **body**
    ```bash
    [
        {
            "_id": "651573a9edb0cafdfaa0c261",
            "title": "testing done",
            "description": "tada tada tada tada",
            "organizerRole": "faculty",
            "organizerEmail": "youremail@gmail.com",
            "organizerName": "test name",
            "status": "upcoming",
            "eventCoordinator": "tatascy",
            "time": "kaya malum",
            "venue": "dekh le",
            "guest": "ek hi hai",
            "__v": 0
        },
        {
            "_id": "6515a9fd1940e85d948f1719",
            "title": "test5",
            "description": "tada5",
            "organizerRole": "faculty",
            "organizerEmail": "chandup@gmail.com",
            "organizerName": "Naresh Test",
            "status": "upcoming",
            "eventCoordinator": "tatascy5",
            "time": "kaya malum 5",
            "venue": "dekh le 5",
            "guest": "ek hi hai 5",
            "__v": 0
        },
        {
            "_id": "652354881610e7f4e7934d2f",
            "title": "test5",
            "description": "tada5",
            "organizerRole": "faculty",
            "organizerEmail": "youremail@gmail.com",
            "organizerName": "test name",
            "status": "upcoming",
            "eventCoordinator": "tatascy5",
            "time": "kaya malum 5",
            "venue": "dekh le 5",
            "guest": "ek hi hai 5",
            "__v": 0
        }
    ]
    ```

- DELETE: /deleteEvent

  Request

  - **body**
    ```bash
    {
        "_id": "651573a9edb0cafdfaa0c261"
    }
    ```

  Request

  - **body**
    ```bash
    event deleted successfully
    ```

- GET: /auth/google

  Request

  - **body**
    ```bash
    {
        "success":true,
        "user":
            {
                "_id":"652358861d76208374088d63",
                "googleId":"108483267303266261519",
                "name":"Naresh Chandanbatve",
                "email":"chandanbatven@gmail.com",
                "isOnboarded":false,"__v":0
        },
        "token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImNoYW5kYW5iYXR2ZW5AZ21haWwuY29tIiwiaWQiOiI2NTIzNTg4NjFkNzYyMDgzNzQwODhkNjMiLCJpYXQiOjE2OTY4MTUyMzksImV4cCI6MTY5NzQyMDAzOX0.lLHj5S8lchfX8lXP2go5Z0m0EpbK08sCized8YrRjSA"
    }
    ```

## /notification

- POST: /send
  Request

  - **body**

    ```bash
    {
        "title": "dusra test 5",
        "message": "kya hota hai to dekh lenge",
        "receiverRole": ["faculty", "admin"]
    }
    ```

  Response

  - **body**
    ```bash
    {
        "msg": "notification sent succesffully"
    }
    ```

- GET: /get

  Response

  - **body**
    ```bash
    {
        "msg": "notification received successfully",
        "notifn": [
            {
                "_id": "65207bae1c2e184d25c94b69",
                "title": "Naresh Test",
                "message": "chandumahajan@gmail.com",
                "senderEmail": "chandumahajan@gmail.com",
                "senderName": "Naresh Test",
                "senderRole": "faculty",
                "receiverRole": [
                    "faculty"
                ],
                "receiverBranch": [],
                "receiverYear": [],
                "createdAt": "2023-10-06T21:27:10.038Z",
                "__v": 0
            },
            {
                "_id": "652081d94d9b80825be571dc",
                "title": "dusra test",
                "message": "kya hota hai to dekh lenge",
                "senderEmail": "chandumahajan@gmail.com",
                "senderName": "Naresh Test",
                "senderRole": "faculty",
                "receiverRole": [
                    "faculty",
                    "admin"
                ],
                "receiverBranch": [],
                "receiverYear": [],
                "createdAt": "2023-10-06T21:53:29.268Z",
                "__v": 0
            },
        ]
    }
    ```

- POST: /delete
  Request

  - **body**

    ```bash
    {
        "_id": "6521b28d862a7c2bc88882ca"
    }
    ```

  Response

  - **body**
    ```bash
    {
        "msg": "notification deleted successfully"
    }
    ```
