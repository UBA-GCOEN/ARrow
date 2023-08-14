import mongoose from "mongoose";


//estimated user schema
const userStudentModel = mongoose.Schema({
     id: { type: String },
     name: { type: String, required: true },
     enrollNo: { type: String, required: true },
     email: { type: String, required: true },
     password: { type: String, required: true },
     branch: { type: String },
     year: { type: String },
     roll: { type: Number },
     mobile: { type: Number }

})

export default mongoose.model("userStudents",userStudentModel);