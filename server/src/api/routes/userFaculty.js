import express from 'express';
import session from '../middlewares/session.js'

const router = express.Router();

import {userFaculty, signup, signin} from "../controllers/userFaculty.js";


router.get("/", userFaculty)
router.post("/signup", signup)
router.post("/signin", session, signin)

export default router;