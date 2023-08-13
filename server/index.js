import express from "express";
import bodyParser from "body-parser";
import mongoose from "mongoose";
import cors from "cors";
import * as dotenv from "dotenv";
dotenv.config();

import indexRoute from "./src/api/routes/index.js";

const PORT = process.env.PORT || 5000;
const CONNECTION_URI = process.env.MONGODB_URI;

const app = express();

app.use(bodyParser.json({ limit: "30mb", extended: true }));
app.use(bodyParser.urlencoded({ limit: "30mb", extended: true }));
app.use(cors());

app.get("/", (req, res) => {
  res.send("test");
});
app.use(indexRoute);

mongoose
  .connect(CONNECTION_URI, { useNewUrlParser: true, useUnifiedTopology: true })
  .then(() => {
    app.listen(PORT, () => {
      console.log("server running on port : " + PORT);
    });
  })
  .catch((err) => console.log(err.message));
