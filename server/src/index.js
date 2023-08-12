import express from "express";
import bodyParser from "body-parser";
import mongoose from "mongoose";
import cors from "cors";
import * as dotenv from "dotenv";
dotenv.config();

const PORT = process.env.PORT || 5000;

const app = express()

import indexRoute from "./api/routes/index.js";
app.use(indexRoute)


app.listen(PORT, () => {
    console.log("server running on port "+PORT)
});
