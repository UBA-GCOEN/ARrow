import express from 'express';
import session from '../middlewares/session.js'
import {csrfProtect} from '../middlewares/csrfProtection.js';

const router = express.Router();

import {user, signup, signin} from "../controllers/user.js";


router.get("/", user)
router.post("/signup", session, signup)
// router.post("/signin", session, csrfProtect, signin)
router.post("/signin", session, signin)

export default router;