import express from 'express';
import session from '../middlewares/session.js'
import {csrfProtect} from '../middlewares/csrfProtection.js';

const router = express.Router();

import {userFaculty, signup, signin} from "../controllers/userFaculty.js";


router.get("/", userFaculty)
router.post("/signup", signup)
router.post("/signin", session, csrfProtect, signin)

export default router;