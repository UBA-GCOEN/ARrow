import express from 'express';
import session from '../middlewares/session.js'

const router = express.Router();

import {userAdmin, signup, signin} from "../controllers/userAdmin.js";
import  { editProfile, profile } from '../controllers/profile.js';

import authAdmin from '../middlewares/authAdmin.js'


router.get("/", profile)
router.post("/edit", session, authAdmin, editProfile)
export default router;