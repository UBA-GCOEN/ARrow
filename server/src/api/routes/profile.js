import express from 'express';
import session from '../middlewares/session.js'

const router = express.Router();

import  { editProfile, profile } from '../controllers/profile.js';

import authUser from '../middlewares/authUser.js'


router.get("/", profile)
router.post("/edit", session, authUser, editProfile)
export default router;