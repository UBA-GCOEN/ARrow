import express from 'express';
import session from '../middlewares/session.js'
import {csrfProtect} from '../middlewares/csrfProtection.js';

const router = express.Router();

import {user, signup, signin} from "../controllers/user.js";


router.get("/", user)
router.post("/signup", signup)
router.post("/signin", session, csrfProtect, signin)

export default router;