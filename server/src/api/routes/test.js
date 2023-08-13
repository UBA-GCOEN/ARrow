import express from 'express';
const router = express.Router();


import testController from "../controllers/test.js";

router.get("/test", testController)

export default router;