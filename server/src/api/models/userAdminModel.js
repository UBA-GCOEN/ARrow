import mongoose from "mongoose";

//estimated admin schema
const userAdminModel = new mongoose.Schema({
  id: { type: String },
  name: { type: String, required: true },
  email: { type: String, required: true },
  password: { type: String, required: true },
  role: { type: String, default: 'admin'},
});

export default mongoose.model("userAdmins", userAdminModel);
