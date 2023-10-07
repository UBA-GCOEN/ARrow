import express from 'express';
import session from '../middlewares/session.js'
import  { updateProfile, profile } from '../controllers/profile.js';
import { csrfProtect } from '../middlewares/csrfProtection.js';
import authUser from '../middlewares/authUser.js'


const router = express.Router();

router.get("/", profile)
router.post("/update", authUser, updateProfile)
export default router;