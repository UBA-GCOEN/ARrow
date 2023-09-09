import jwt from "jsonwebtoken";
import * as dotenv from "dotenv";
dotenv.config();

const generateToken = (result, SECRET, tokenValidity = "1h") => {
  return jwt.sign({ email: result.email, id: result._id, role: result.role}, SECRET, {
    expiresIn: tokenValidity,
  });
};

export default generateToken;