import express from 'express';
const router = express.Router();
import session from '../middlewares/session.js';
import {csrfProtect} from '../middlewares/csrfProtection.js';

import {userStudent, signup, signin} from "../controllers/userStudent.js";


router.get("/", userStudent)
router.post("/signup", signup)
router.post("/signin", session, csrfProtect, signin)

export default router;