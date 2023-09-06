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
import userStaff from "./src/api/routes/userStaff.js";
import userVisitor from "./src/api/routes/userVisitor.js";
import userFaculty from "./src/api/routes/userFaculty.js";

app.use("/", indexRoute)
app.use("/test", testRoute)
app.use("/userStudent", userStudent)
app.use("/userStaff", userStaff)
app.use("/userAdmin", userAdmin)
app.use("/userVisitor", userVisitor)
app.use("/userFaculty", userFaculty)



mongoose
  .connect(CONNECTION_URI, { useNewUrlParser: true, useUnifiedTopology: true })
  .then(() => {
    app.listen(PORT, () => {
      console.log("server running on port : " + PORT);
    });
  })
  .catch((err) => console.log(err.message));
