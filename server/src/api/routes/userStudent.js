import express from 'express';
const router = express.Router();

import {userStudent, signup} from "../controllers/userStudent.js";


router.get("/", userStudent)
router.post("/signup", signup)

export default router;