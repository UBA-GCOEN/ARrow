import express from "express";
import bodyParser from "body-parser";
import mongoose from "mongoose";
import cors from "cors";
// import rateLimit from 'express-rate-limit';
import csrf from 'csurf';
import * as dotenv from "dotenv";
dotenv.config();
import initializePassport from './src/api/middlewares/passportConfig.js'

const PORT = process.env.PORT || 5000;
const CONNECTION_URI = process.env.MONGODB_URI;

const app = express();

app.use(bodyParser.json({ limit: "30mb", extended: true }));
app.use(bodyParser.urlencoded({ limit: "30mb", extended: true }));
app.use(cors());


initializePassport(app)



import indexRoute from "./src/api/routes/index.js";
import testRoute from "./src/api/routes/test.js";
import user from "./src/api/routes/user.js";
import profile from "./src/api/routes/profile.js";
import event from "./src/api/routes/events.js";
import googleAuth from "./src/api/routes/googleAuth.js"



//rate limiter
// const limiter = rateLimit({
//   windowMs: 15 * 60 * 1000, // 15 minutes
//   max: 100, // limit each IP to 100 requests per windowMs
// });


// app.use("/", limiter, indexRoute)
// app.use("/test", limiter, testRoute)
// app.use("/user", limiter, user)
// app.use("/profile", limiter, profile)
// app.use("/event", limiter, event)

app.use("/", indexRoute)
app.use("/test", testRoute)
app.use("/user", user)
app.use("/profile", profile)
app.use("/event", event)
app.use("/auth",googleAuth)


app.use(csrf)


mongoose
  .connect(CONNECTION_URI, { useNewUrlParser: true, useUnifiedTopology: true })
  .then(() => {
    app.listen(PORT, () => {
      console.log("server running on port : " + PORT);
    });
  })
  .catch((err) => console.log(err.message));
