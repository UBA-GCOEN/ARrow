import express from "express";
import bodyParser from "body-parser";
import mongoose from "mongoose";
import cors from "cors";
import * as dotenv from "dotenv";
dotenv.config();


const PORT = process.env.PORT || 5000;
const CONNECTION_URI = process.env.MONGODB_URI;

const app = express();

app.use(bodyParser.json({ limit: "30mb", extended: true }));
app.use(bodyParser.urlencoded({ limit: "30mb", extended: true }));
app.use(cors());

import indexRoute from "./src/api/routes/index.js";
import testRoute from "./src/api/routes/test.js";
import userStudent from "./src/api/routes/userStudent.js";
import userAdmin from "./src/api/routes/userAdmin.js";

app.use("/", indexRoute);
app.use("/test", testRoute);
app.use("/userStudent", userStudent)
app.use("/userAdmin", userAdmin)


mongoose
  .connect(CONNECTION_URI, { useNewUrlParser: true, useUnifiedTopology: true })
  .then(() => {
    app.listen(PORT, () => {
      console.log("server running on port : " + PORT);
    });
  })
  .catch((err) => console.log(err.message));
