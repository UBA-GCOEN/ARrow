import express from 'express';
import session from '../middlewares/session.js'
import {csrfProtect} from '../middlewares/csrfProtection.js';

const router = express.Router();

import {userStaff, signup, signin} from "../controllers/userStaff.js";


router.get("/", userStaff)
router.post("/signup", signup)
router.post("/signin", session, csrfProtect, signin)

export default router;