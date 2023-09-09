import express from 'express';
import session from '../middlewares/session.js'

const router = express.Router();

import {userAdmin, signup, signin} from "../controllers/userAdmin.js";
import editProfile from '../controllers/editProfile.js';
import authAdmin from '../middlewares/authAdmin.js'


// router.get("/", profile)
// router.get("/edit", profile)
router.post("/edit", authAdmin, editProfile)
export default router;