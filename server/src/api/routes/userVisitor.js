import express from 'express';
import session from '../middlewares/session.js'
import {csrfProtect} from '../middlewares/csrfProtection.js';

const router = express.Router();

import {userVisitor, signup, signin} from "../controllers/userVisitor.js";


router.get("/", userVisitor)
router.post("/signup", signup)
router.post("/signin", session, csrfProtect, signin)

export default router;