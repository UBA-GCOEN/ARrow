import express from 'express';
const router = express.Router();
import session from '../middlewares/session.js';

import {userStudent, signup, signin} from "../controllers/userStudent.js";


router.get("/", userStudent)
router.post("/signup", signup)
router.post("/signin",session, signin)

export default router;