import express from "express";
import bodyParser from "body-parser";
import mongoose from "mongoose";
import cors from "cors";
import * as dotenv from "dotenv";
dotenv.config();

const PORT = process.env.PORT || 5000;
const  CONNECTION_URI = process.env.MONGODB_URI;

const app = express()
app.use(cors)

import indexRoute from "./api/routes/index.js";
app.use(indexRoute)

mongoose
.connect(CONNECTION_URI,  { useNewUrlParser: true, useUnifiedTopology: true })
.then(()=>{
    app.listen(PORT, () => {
        console.log("server running on port "+PORT)
    })
})
.catch((err) => console.log(err.message));

