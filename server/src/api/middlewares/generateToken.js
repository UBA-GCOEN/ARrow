import jwt from "jsonwebtoken";
import * as dotenv from "dotenv";
dotenv.config();

const generateToken = (result, SECRET, tokenValidity = "7d") => {
  return jwt.sign({ email: result.email, id: result._id }, SECRET, {
    expiresIn: tokenValidity,
  });
};

export default generateToken;