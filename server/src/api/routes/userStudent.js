import express from 'express';
const router = express.Router();

import {userStudent, signup, signin} from "../controllers/userStudent.js";


router.get("/", userStudent)
router.post("/signup", signup)
router.post("/signin", signin)

export default router;