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
  User deleted successffuly
  ```

## DELETE /deleteuser

Response

- **body**
  ```bash
  User deleted successffuly
  ```

## POST /sendEmail

Response

- **body**
  ```bash
  User deleted successffuly
  ```
