import express from "express"
import session from 'express-session'
import * as dotenv from "dotenv";
dotenv.config();


const app = express();
app.use(session({
  secret: process.env.SESSION_SECRET,
  resave: false,
  saveUninitialized: false
},
 (next)=>{
  next()
 }
))

export default app

