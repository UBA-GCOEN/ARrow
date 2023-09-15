import express from 'express';
import session from '../middlewares/session.js'
import  { editProfile, profile } from '../controllers/profile.js';
import { csrfProtect } from '../middlewares/csrfProtection.js';
import authUser from '../middlewares/authUser.js'


const router = express.Router();

router.get("/", profile) //to add get profile controller logic
router.post("/edit", session, csrfProtect, authUser, editProfile)
export default router;